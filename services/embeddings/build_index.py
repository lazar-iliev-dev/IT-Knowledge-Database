# services/embeddings/build_index.py
import json
import requests
import numpy as np
from sentence_transformers import SentenceTransformer
import faiss

API_URL = "http://localhost:5249/api/articles"  # hole Artikel vom .NET Backend
MODEL = "sentence-transformers/all-MiniLM-L6-v2"

def load_articles():
    r = requests.get(API_URL)
    r.raise_for_status()
    return r.json()

def build():
    articles = load_articles()
    texts = [ (a["id"], a["title"] + "\n" + a["content"]) for a in articles ]  # id as string (GUID)
    model = SentenceTransformer(MODEL)
    embeddings = model.encode([t[1] for t in texts], show_progress_bar=True, convert_to_numpy=True)

    dim = embeddings.shape[1]
    index = faiss.IndexFlatIP(dim)  # cosine? use normalized vectors and inner product
    faiss.normalize_L2(embeddings)
    index.add(embeddings)
    faiss.write_index(index, "index.faiss")

    # Save mapping id->pos and metadata
    meta = {"ids": [t[0] for t in texts], "titles":[t[1] for t in texts]}
    with open("meta.json","w", encoding="utf-8") as f:
        json.dump(meta, f, ensure_ascii=False, indent=2)

if __name__ == "__main__":
    build()
