import { useState, useEffect } from 'react';

export default function App() {
  const [articles, setArticles] = useState([]);
  const [selected, setSelected] = useState(null);
  const [q, setQ] = useState('');

  useEffect(() => {
    fetch('http://localhost:5249/api/articles')
      .then(r => r.json())
      .then(setArticles);
  }, []);

  const search = async () => {
    const res = await fetch(`http://localhost:5249/api/articles?search=${encodeURIComponent(q)}`);
    setArticles(await res.json());
    setSelected(null);
  };

  return (
    <main style={{ maxWidth: 600, margin: '2rem auto', fontFamily: 'sans-serif' }}>
      <h1>IT-Support Wissensdatenbank</h1>
      <input
        value={q}
        onChange={e => setQ(e.target.value)}
        placeholder="Suche..."
        style={{ width: '100%', padding: '0.5rem', marginBottom: '1rem' }}
      />
      <button onClick={search} style={{ padding: '0.5rem 1rem', marginBottom: '1rem' }}>
        Suche
      </button>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {articles.map(a => (
          <li
            key={a.id}
            onClick={() => setSelected(a)}
            style={{ padding: '0.5rem 0', borderBottom: '1px solid #ddd', cursor: 'pointer' }}
          >
            {a.title}
          </li>
        ))}
      </ul>
      {selected && (
        <article style={{ marginTop: '2rem' }}>
          <h2>{selected.title}</h2>
          <p style={{ whiteSpace: 'pre-wrap' }}>{selected.content}</p>
        </article>
      )}
    </main>
  );
}
