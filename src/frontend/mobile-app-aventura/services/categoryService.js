import { apiFetch } from './apiClient';

export async function fetchCategories() {
  const response = await apiFetch('/api/Genero');

  if (!response.ok) {
    throw new Error('Não foi possível carregar as categorias');
  }

  const generos = await response.json();
  
  // O backend retorna um array de strings. Mapeamos para id e label.
  const mappedCategories = generos.map(g => ({
    id: g.toLowerCase(),
    label: g
  }));

  // Retornamos "Todos" como primeira categoria
  return [
    { id: 'all', label: 'Todos' },
    ...mappedCategories
  ];
}
