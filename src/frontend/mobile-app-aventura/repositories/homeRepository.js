import {
  heroBanners,
  categories,
  trendingEvents,
  recommendedEvents,
  newEvents,
  nearbyEvents,
  theaters,
} from '../data/mockHomeData';

import { API_BASE_URL } from '../config/api';

export function getHomeData() {
  return {
    heroBanners,
    categories,
    trendingEvents,
    recommendedEvents,
    newEvents,
    nearbyEvents,
    theaters,
  };
}

function mapRecomendacaoToEvent(item) {
  return {
    id: item.espetaculo.id,
    showId: item.espetaculo.id,
    title: item.espetaculo.nome,
    location: item.espetaculo.tipoEvento,
    categoryId: item.espetaculo.tipoEvento,
    image: null,
  };
}

export async function getRecommendedEvents(userId) {
  try {
    const response = await fetch(
      `${API_BASE_URL}/api/recomendacao/${userId}?limit=10`
    );

    if (!response.ok) {
      return recommendedEvents;
    }

    const data = await response.json();

    if (!Array.isArray(data) || data.length === 0) {
      return [];
    }

    return data.map(mapRecomendacaoToEvent);
  } catch {
    return recommendedEvents;
  }
}