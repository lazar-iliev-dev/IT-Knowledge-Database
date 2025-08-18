// frontend/src/components/ArticleForm.jsx
import { useEffect, useState } from "react";

export default function ArticleForm({ onSubmit, editing, cancelEdit }) {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");

  // Wenn "Bearbeiten" aktiv ist, Felder mit Artikeldaten füllen
  useEffect(() => {
    if (editing) {
      setTitle(editing.title);
      setContent(editing.content);
    } else {
      setTitle("");
      setContent("");
    }
  }, [editing]);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!title.trim() || !content.trim()) return;

    const article = editing
      ? { ...editing, title, content }
      : { title, content };

    onSubmit(article);
    setTitle("");
    setContent("");
  };

  return (
    <form onSubmit={handleSubmit} className="p-4 flex flex-col gap-2 border rounded mb-4">
      <input
        type="text"
        placeholder="Titel"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        className="border rounded p-2"
      />
      <textarea
        placeholder="Inhalt"
        value={content}
        onChange={(e) => setContent(e.target.value)}
        className="border rounded p-2"
      />
      <div className="flex gap-2">
        <button type="submit" className="bg-blue-600 text-white p-2 rounded">
          {editing ? "Aktualisieren" : "Speichern"}
        </button>
        {editing && (
          <button
            type="button"
            onClick={cancelEdit}
            className="bg-gray-400 text-white p-2 rounded"
          >
            Abbrechen
          </button>
        )}
      </div>
    </form>
  );
}
