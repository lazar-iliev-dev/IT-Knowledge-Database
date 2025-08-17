// frontend/src/App.jsx
import { useEffect, useState } from "react";
import ArticleForm from "./components/ArticleForm";
import ArticleList from "./components/ArticleList";

function App() {
  const [articles, setArticles] = useState([]);
  const [editing, setEditing] = useState(null);

  const loadArticles = () => {
    fetch("http://localhost:5249/api/articles")
      .then((res) => res.json())
      .then(setArticles)
      .catch((err) => console.error("Fehler beim Laden:", err));
  };

  useEffect(() => {
    loadArticles();
  }, []);

  const handleNewArticle = async (article) => {
    await fetch("http://localhost:5249/api/articles", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(article),
    });
    loadArticles();
  };

  const handleUpdateArticle = async (article) => {
    await fetch(`http://localhost:5249/api/articles/${article.id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(article),
    });
    setEditing(null);
    loadArticles();
  };

  const handleDeleteArticle = async (id) => {
    await fetch(`http://localhost:5249/api/articles/${id}`, {
      method: "DELETE",
    });
    loadArticles();
  };

  return (
    <div className="max-w-2xl mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">📚 IT Knowledge Base</h1>

      <ArticleForm
        onSubmit={editing ? handleUpdateArticle : handleNewArticle}
        editing={editing}
        cancelEdit={() => setEditing(null)}
      />

      <ArticleList
        articles={articles}
        onEdit={(a) => setEditing(a)}
        onDelete={handleDeleteArticle}
      />
    </div>
  );
}

export default App;
