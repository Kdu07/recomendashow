import { novicaRebeldeDetail } from '../data/mockEventDetailData';

const mockDatabase = {
  'trending-novica': novicaRebeldeDetail,
};

export function getEventDetail(id) {
  return mockDatabase[id] ?? null;
}
