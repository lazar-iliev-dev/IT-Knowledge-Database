# services/embeddings/app.py
import os, json
from fastapi import FastAPI, HTTPException, Query
from pydantic import BaseModel
import faiss, numpy as np
from sentence_transformers import SentenceTransformer

try:
    import faiss
    FAISS_AVAILABLE = True
except ImportError:
    FAISS_AVAILABLE = False

app = FastAPI()
MODEL = "sentence-transformers/all-MiniLM-L6-v2"
model = SentenceTransformer(MODEL)
index = None
meta = {"ids": [], "titles": []}

@app.on_event("startup")
def load_index():
    global index, meta
    if FAISS_AVAILABLE and os.path.exists("index.faiss") and os.path.exists("meta.json"):
        index = faiss.read_index("index.faiss")
        with open("meta.json", "r", encoding="utf-8") as f:
            meta = json.load(f)
        print("Loaded FAISS index with", len(meta["ids"]), "docs")
    else:
        index = None
        meta = {"ids": [], "titles": []}
        print("No FAISS index found → running fallback mode")

#@app.get("/search")
#def search(q: str = Query(...), k: int = 5):
 #   emb = model.encode(q, convert_to_numpy=True)
 #   faiss.normalize_L2(emb)
 #   D, I = index.search(np.array([emb]), k)
 #   results = []
 #   for score, idx in zip(D[0], I[0]):
 #       if idx < 0: continue
 #       aid = meta["ids"][idx]
 #       results.append({"articleId": aid, "score": float(score)})
 #   return {"query": q, "results": results}

@app.get("/search")
def search(q: str, k: int = 5):
    if index is None:
        # Fallback: einfache Textsuche
        results = []
        for i, t in enumerate(meta["titles"]):
            if q.lower() in t.lower():
                results.append({"articleId": meta["ids"][i], "score": 1.0})
                if len(results) >= k:
                    break
        return {"query": q, "results": results}

    # normal mit FAISS suchen ...
