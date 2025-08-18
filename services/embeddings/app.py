# services/embeddings/app.py
from fastapi import FastAPI, HTTPException, Query
from pydantic import BaseModel
import faiss, json, numpy as np
from sentence_transformers import SentenceTransformer

app = FastAPI()
MODEL = "sentence-transformers/all-MiniLM-L6-v2"
model = SentenceTransformer(MODEL)
index = None
meta = None

@app.on_event("startup")
def load_index():
    global index, meta
    index = faiss.read_index("index.faiss")
    with open("meta.json","r", encoding="utf-8") as f:
        meta = json.load(f)

@app.get("/search")
def search(q: str = Query(...), k: int = 5):
    emb = model.encode(q, convert_to_numpy=True)
    faiss.normalize_L2(emb)
    D, I = index.search(np.array([emb]), k)
    results = []
    for score, idx in zip(D[0], I[0]):
        if idx < 0: continue
        aid = meta["ids"][idx]
        results.append({"articleId": aid, "score": float(score)})
    return {"query": q, "results": results}
