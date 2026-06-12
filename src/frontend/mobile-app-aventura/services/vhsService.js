const cache = new Map();

export const CACHE_STATUS = {
  HIT: 'HIT',
  MISS: 'MISS',
};

let lastCacheStatus = null;

export async function cachedFetch(url, options = {}) {
  const {
    successTtl = 24 * 60 * 60 * 1000,
    notFoundTtl = 5 * 60 * 1000,
  } = options;

  const cachedEntry = cache.get(url);

  if (
    cachedEntry &&
    cachedEntry.expiresAt > Date.now()
  ) {
    lastCacheStatus = CACHE_STATUS.HIT;
    return cachedEntry.data;
  }

  lastCacheStatus = CACHE_STATUS.MISS;

  let response;

  try {
    response = await fetch(url);
  } catch {
    throw new Error('Não foi possível consultar o CEP agora.');
  }

  if (!response.ok) {
    throw new Error('Não foi possível consultar o CEP agora.');
  }

  let data;

  try {
    data = await response.json();
  } catch {
    throw new Error('Não foi possível consultar o CEP agora.');
  }

  const ttl = data?.erro
    ? notFoundTtl
    : successTtl;

  cache.set(url, {
    data,
    expiresAt: Date.now() + ttl,
  });

  return data;
}

export function flushCache() {
  cache.clear();
  lastCacheStatus = null;
}

export function cacheSize() {
  return cache.size;
}

export function getLastCacheStatus() {
  return lastCacheStatus;
}
