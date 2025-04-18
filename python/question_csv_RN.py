from langchain_community.document_loaders import WebBaseLoader
from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain_chroma import Chroma
from langchain_ollama import OllamaEmbeddings
from langchain_core.output_parsers import StrOutputParser
from langchain_core.prompts import ChatPromptTemplate
from langchain_ollama import ChatOllama
import pandas as pd
from langchain_experimental.agents.agent_toolkits import create_csv_agent

text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=0)

model = ChatOllama(
    model="deepseek-r1",
)

# Reading a CSV file
csv_file_path = 'C:\\Users\\rnavarr\\repos\\Innovation-Week-2025\\data\\features.csv'
df_csv = pd.read_csv(csv_file_path)

# Create the CSV agent
agent = create_csv_agent(model, csv_file_path,
                         verbose=True, allow_dangerous_code=True)


def query_data(query):
    response = agent.invoke(query)
    return response


query = "how many rows are there?"
# query = "what are the columns of the csv about, give a small description?"
#query = "Does Ismael hows any features? Which ones?"
#query = "how many features are in this file? how many owners are in this file?"


response = query_data(query)
# print(response)
print(response['output'])