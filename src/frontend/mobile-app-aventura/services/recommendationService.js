import { apiFetch } from './apiClient';
import { mapDtoToHomeEvent } from './showService';

export async function fetchRecommendations(userId) {
  if (!userId) return [];

  const response = await apiFetch(`/api/Recomendacao/${userId}?limit=5`);

  if (!response.ok) {
    console.warn('Não foi possível carregar as recomendações personalizadas.');
    return [];
  }

  const recommendations = await response.json();
  
  // O endpoint retorna RecomendacaoItemDto: { Espetaculo, Score, Motivo }
  return recommendations.map(rec => {
    const event = mapDtoToHomeEvent(rec.Espetaculo ?? rec.espetaculo);
    // Podemos adicionar informações de motivo da recomendação no evento, caso seja exibido
    return {
      ...event,
      recommendationReason: rec.Motivo ?? rec.motivo,
      recommendationScore: rec.Score ?? rec.score
    };
  });
}
