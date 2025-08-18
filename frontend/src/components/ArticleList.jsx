// frontend/src/components/ArticleList.jsx
export default function ArticleList({ articles, onEdit, onDelete }) {
  return (
    <div className="p-4 border rounded">
      <h2 className="font-bold text-xl mb-2">Artikel</h2>
      <ul className="space-y-2">
        {articles.length === 0 && <li className="text-gray-500">Keine Artikel vorhanden</li>}
        {articles.map((a) => (
          <li key={a.id} className="border p-2 rounded flex justify-between items-start">
            <div>
              <h3 className="font-semibold">{a.title}</h3>
              <p>{a.content}</p>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => onEdit(a)}
                className="bg-yellow-500 text-white px-2 py-1 rounded"
              >
                ✏️ Bearbeiten
              </button>
              <button
                onClick={() => onDelete(a.id)}
                className="bg-red-600 text-white px-2 py-1 rounded"
              >
                🗑️ Löschen
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}

