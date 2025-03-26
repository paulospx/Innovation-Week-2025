import os
import pandas as pd
import numpy as np
from typing import Dict, Any

from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain_chroma import Chroma
from langchain_ollama import OllamaEmbeddings
from langchain_core.output_parsers import StrOutputParser
from langchain_core.prompts import ChatPromptTemplate
from langchain_ollama import ChatOllama
from langchain_experimental.agents.agent_toolkits import create_csv_agent

class ExcelDataProcessor:
    def __init__(self, file_path: str, max_rows: int = 1000, chunk_size: int = 500):
        self.file_path = file_path
        self.max_rows = max_rows
        self.chunk_size = chunk_size
        
        # Configure Ollama model
        self.llm = ChatOllama(
            model="deepseek-r1",  # Specify your Ollama model name
            temperature=0.2  # Low temperature for more deterministic responses
        )
        
        self.sheet_data = {}
        self.agents = {}
    
    def _validate_sheet(self, df: pd.DataFrame) -> pd.DataFrame:
        df = df.dropna(how='all', axis=1)
        df = df.dropna(how='all')
        df = df.loc[:, ~df.columns.str.contains('^Unnamed:', case=False)]
        df.columns = [
            col.strip().lower().replace(' ', '_') 
            if pd.notna(col) else f'column_{i}' 
            for i, col in enumerate(df.columns)
        ]
        return df
    
    def load_excel_with_filters(self):
        try:
            xls = pd.ExcelFile(self.file_path, engine='openpyxl')
            total_processed_rows = 0
            for sheet_name in xls.sheet_names:
                try:
                    df = pd.read_excel(
                        xls, 
                        sheet_name=sheet_name, 
                        dtype=object
                    )
                    df = self._validate_sheet(df)
                    if df.empty:
                        continue
                    if total_processed_rows + len(df) > self.max_rows:
                        df = df.head(self.max_rows - total_processed_rows)
                    total_processed_rows += len(df)
                    self.sheet_data[sheet_name] = df
                    if total_processed_rows >= self.max_rows:
                        break
                except Exception as sheet_error:
                    print(f"Error processing sheet {sheet_name}: {sheet_error}")
        except Exception as e:
            print(f"Critical error reading Excel file: {e}")
            raise
    
    def create_sheet_agents(self):
        for sheet_name, df in self.sheet_data.items():
            try:
                # Save DataFrame to CSV file
                csv_file_path = f"{sheet_name}.csv"
                df.to_csv(csv_file_path, index=False)
                
                # Create agent using the CSV file path
                agent = create_csv_agent(
                    self.llm,
                    csv_file_path,
                    verbose=True,
                    allow_dangerous_code=True
                )
                agent.metadata = {
                    'sheet_name': sheet_name,
                    'rows': len(df),
                    'columns': list(df.columns)
                }
                self.agents[sheet_name] = agent
            except Exception as agent_error:
                print(f"Could not create agent for {sheet_name}: {agent_error}")
    
    def query_sheets(self, query: str) -> Dict[str, Any]:
        results = {}
        for sheet_name, agent in self.agents.items():
            try:
                response = agent.invoke(query)
                results[sheet_name] = {
                    'response': response,
                    'metadata': agent.metadata
                }
            except Exception as e:
                results[sheet_name] = {
                    'error': str(e),
                    'metadata': agent.metadata
                }
        return results
    
    def summarize_dataset(self) -> str:
        summary = "Dataset Overview:\n"
        summary += f"Total Sheets: {len(self.sheet_data)}\n"
        summary += "Sheet Details:\n"
        for sheet_name, df in self.sheet_data.items():
            summary += f"- {sheet_name}: {len(df)} rows, {len(df.columns)} columns\n"
        return summary

def main():
    processor = ExcelDataProcessor(
        r'C:\Users\rnavarr\repos\Innovation-Week-2025\data\total overview.xlsx',
        max_rows=5000,
        chunk_size=500
    )
    
    processor.load_excel_with_filters()
    processor.create_sheet_agents()
    
    queries = [
        "tell me based on this sheet : what number is the verdi model?",
        "tell me who is the model owner of the verdi model",
        "tell me when is due to migration",
        "tell me if this model has a python preprocessor"
    ]
    
    for query in queries:
        print(f"\nQuery: {query}")
        results = processor.query_sheets(query)
        for sheet, result in results.items():
            print(f"\nSheet: {sheet}")
            if 'response' in result:
                print(result['response'])
            elif 'error' in result:
                print(f"Error: {result['error']}")

if __name__ == "__main__":
    main()
