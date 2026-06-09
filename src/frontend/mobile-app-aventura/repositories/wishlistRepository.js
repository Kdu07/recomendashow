import { apiClient } from '../services/apiClient';
import {
  trendingEvents,
  recommendedEvents,
  newEvents,
  nearbyEvents,
} from '../data/mockHomeData';
import {
  WISHLIST_EVENT_BACKEND_IDS,
  WISHLIST_USER_ID,
} from '../config/wishlistIntegrationConfig';
import { getCurrentUserId } from '../services/userSession';

const DEFAULT_EVENT_IMAGE = require('../assets/home/mamma-mia.png');

const localCatalog = [
  ...trendingEvents,
  ...recommendedEvents,
  ...newEvents,
  ...nearbyEvents,
].reduce((items, event) => {
  const backendId = getBackendIdForEvent(event);

  if (!backendId || items.some((item) => item.backendId === backendId)) {
    return items;
  }

  return [
    ...items,
    {
      ...event,
      backendId,
    },
  ];
}, []);

function getBackendIdForEvent(event) {
  return event?.backendId ||
    WISHLIST_EVENT_BACKEND_IDS[event?.id] ||
    WISHLIST_EVENT_BACKEND_IDS[event?.showId];
}

function getLocalEventBackendId(eventId) {
  return localCatalog.find((event) => event.id === eventId)?.backendId;
}

function getLocalEventByBackendId(backendId) {
  return localCatalog.find((event) => event.backendId === backendId);
}

function mapEspetaculoDtoToEvent(espetaculo) {
  const backendId = espetaculo.id || espetaculo.Id;
  const nome = espetaculo.nome || espetaculo.Nome;
  const genero = espetaculo.genero || espetaculo.Genero;
  const localEvent = getLocalEventByBackendId(backendId);

  return {
    ...(localEvent || {}),
    id: localEvent?.id || backendId,
    backendId,
    title: nome || localEvent?.title,
    categoryId: String(genero || localEvent?.categoryId || '').toLowerCase(),
    location: localEvent?.location || 'Aventura',
    city: localEvent?.city || 'Brasil',
    image: localEvent?.image || DEFAULT_EVENT_IMAGE,
  };
}

async function fetchWishlistEvents() {
  const usuarioId = getWishlistUserId();

  const espetaculos = await apiClient.get(`/Wishlist/${usuarioId}`);
  return (espetaculos || []).map(mapEspetaculoDtoToEvent);
}

export async function getWishlistData() {
  const events = await fetchWishlistEvents();

  return {
    events,
  };
}

export async function saveWishlistData(events) {
  return events;
}

export async function addWishlistEvent(event) {
  const backendId = getBackendIdForEvent(event);
  const usuarioId = getWishlistUserId();

  if (!backendId) {
    throw new Error('Espetaculo sem mapeamento para a API.');
  }

  await apiClient.post(`/Wishlist/${usuarioId}/${backendId}`);
  return fetchWishlistEvents();
}

export async function removeWishlistEvent(eventId) {
  const usuarioId = getWishlistUserId();

  const backendId = eventId && eventId.length === 36
    ? eventId
    : WISHLIST_EVENT_BACKEND_IDS[eventId] || getLocalEventBackendId(eventId);

  if (!backendId) {
    throw new Error('Espetaculo sem mapeamento para a API.');
  }

  await apiClient.delete(`/Wishlist/${usuarioId}/${backendId}`);
  return fetchWishlistEvents();
}

export async function toggleWishlistEvent(event) {
  const backendId = getBackendIdForEvent(event);
  const usuarioId = getWishlistUserId();

  if (!backendId) {
    throw new Error('Espetaculo sem mapeamento para a API.');
  }

  const isSaved = await checkEventIsSaved(backendId);

  if (isSaved) {
    await apiClient.delete(`/Wishlist/${usuarioId}/${backendId}`);
  } else {
    await apiClient.post(`/Wishlist/${usuarioId}/${backendId}`);
  }

  return fetchWishlistEvents();
}

export async function checkEventIsSaved(eventId) {
  const usuarioId = getWishlistUserId();

  const backendId = eventId && eventId.length === 36
    ? eventId
    : WISHLIST_EVENT_BACKEND_IDS[eventId] || getLocalEventBackendId(eventId);

  if (!backendId) {
    return false;
  }

  return apiClient.get(`/Wishlist/${usuarioId}/contains/${backendId}`);
}

export function getWishlistEventKey(event) {
  return getBackendIdForEvent(event) || event?.id;
}

function getWishlistUserId() {
  const usuarioId = getCurrentUserId() || WISHLIST_USER_ID;

  if (!usuarioId || usuarioId === 'configure-user-id-here') {
    throw new Error('Configure WISHLIST_USER_ID para testar a Wishlist.');
  }

  return usuarioId;
}
