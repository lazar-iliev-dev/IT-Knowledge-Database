// frontend/src/pages/Home.jsx
import { useEffect, useState } from "react";
import { fetchArticles } from "../services/api";
import SearchBar from "../components/SearchBar";
import ArticleList from "../components/ArticleList";

export default function Home() {
  const [articles, setArticles] = useState([]);
  const [filtered, setFiltered] = useState([]);

  useEffect(() => {
    fetchArticles().then((data) => {
      // Alphabetisch sortieren
      const sorted = data.sort((a, b) =>
        a.title.localeCompare(b.title, "de", { sensitivity: "base" })
      );
      setArticles(sorted);
      setFiltered(sorted);
    });
  }, []);

  const handleSearch = (query) => {
    if (!query) {
      setFiltered(articles);
    } else {
      setFiltered(
        articles.filter((a) =>
          a.title.toLowerCase().includes(query.toLowerCase())
        )
      );
    }
  };

  return (
    <div className="max-w-5xl mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4 text-center">
        📚 IT Support Knowledge Base
      </h1>
      <SearchBar onSearch={handleSearch} />
      <div className="mb-6 flex justify-end">
        <button className="px-4 py-2 bg-blue-600 text-white rounded-lg shadow hover:bg-blue-700">
          ➕ Neuer Artikel
        </button>
      </div>
      <ArticleList articles={filtered} />
    </div>
  );
}
