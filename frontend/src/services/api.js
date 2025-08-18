// frontend/src/services/api.js
const API_URL = "http://localhost:5249/api/articles"; // dein Backend-Endpoint

export async function fetchArticles() {
  const response = await fetch(API_URL);
  if (!response.ok) throw new Error("Fehler beim Laden der Artikel");
  return response.json();
}
