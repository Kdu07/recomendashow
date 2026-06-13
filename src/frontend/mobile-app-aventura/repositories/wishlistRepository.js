import { apiFetch } from '../services/apiClient';
import { getAuthSession } from '../services/tokenStorageService';
import { mapDtoToHomeEvent } from '../services/showService';

async function getUserId() {
  const session = await getAuthSession();
  const userId = session?.usuarioId ?? session?.UsuarioId ?? session?.id;
  if (!userId) throw new Error('Sessao de usuario nao encontrada.');
  return userId;
}

async function readWishlistEvents(usuarioId) {
  const response = await apiFetch(`/api/Wishlist/${usuarioId}`);

  if (!response.ok) {
    const text = await response.text();
    throw new Error(`Erro ao carregar wishlist: ${response.status} ${text}`);
  }

  const dto = await response.json();
  return (Array.isArray(dto) ? dto : []).map(mapDtoToHomeEvent);
}

export async function getWishlistData() {
  const usuarioId = await getUserId();
  const events = await readWishlistEvents(usuarioId);

  return { events };
}

export async function saveWishlistData() {
  return true;
}

export async function addWishlistEvent(event) {
  const usuarioId = await getUserId();
  const showId = event.showId ?? event.id;
  const response = await apiFetch(`/api/Wishlist/${usuarioId}/${showId}`, {
    method: 'POST',
  });

  if (!response.ok && response.status !== 409) {
    const text = await response.text();
    throw new Error(`Erro ao adicionar a wishlist: ${response.status} ${text}`);
  }

  return readWishlistEvents(usuarioId);
}

export async function removeWishlistEvent(eventId) {
  const usuarioId = await getUserId();
  const response = await apiFetch(`/api/Wishlist/${usuarioId}/${eventId}`, {
    method: 'DELETE',
  });

  if (!response.ok && response.status !== 404) {
    const text = await response.text();
    throw new Error(`Erro ao remover da wishlist: ${response.status} ${text}`);
  }

  return readWishlistEvents(usuarioId);
}

export async function checkEventIsSaved(eventId) {
  const usuarioId = await getUserId();
  const response = await apiFetch(`/api/Wishlist/${usuarioId}/contains/${eventId}`);

  if (!response.ok) {
    return false;
  }

  return response.json();
}

export async function toggleWishlistEvent(event) {
  const showId = event.showId ?? event.id;
  const saved = await checkEventIsSaved(showId);

  if (saved) {
    return removeWishlistEvent(showId);
  }

  return addWishlistEvent(event);
}
