import { useEffect, useState } from "react";
import ArticleForm from "./components/ArticleForm";
import ArticleList from "./components/ArticleList";
import SearchBar from "./components/SearchBar";

function App() {
  const [articles, setArticles] = useState([]);
  const [editing, setEditing] = useState(null);

  const loadArticles = async (filters = {}) => {
    let url = "http://localhost:5249/api/articles";
    const params = new URLSearchParams(filters).toString();
    if (params) url += `?${params}`;

    const res = await fetch(url);
    setArticles(await res.json());
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
    <div className="max-w-3xl mx-auto p-6">
      <h1 className="text-3xl font-bold mb-6">📚 IT Knowledge Base</h1>

      <SearchBar onSearch={loadArticles} />

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
// This is the main application component that manages the state and renders the UI.
// It includes the article form, article list, and search bar components.
// It handles loading articles, creating, updating, and deleting articles via API calls.
// The application uses React hooks for state management and side effects.