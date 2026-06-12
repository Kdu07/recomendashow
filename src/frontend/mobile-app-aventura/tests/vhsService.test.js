import {
  cachedFetch,
  cacheSize,
  flushCache,
  getLastCacheStatus,
} from '../services/vhsService';

function okJson(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('vhsService', () => {
  beforeEach(() => {
    flushCache();
    global.fetch = jest.fn();
  });

  afterEach(() => {
    flushCache();
    jest.restoreAllMocks();
  });

  test('stores a successful response as MISS and returns it', async () => {
    const payload = { cep: '01310-100' };
    global.fetch.mockResolvedValue(okJson(payload));

    await expect(cachedFetch('https://example.test/cep')).resolves.toEqual(payload);

    expect(global.fetch).toHaveBeenCalledTimes(1);
    expect(cacheSize()).toBe(1);
    expect(getLastCacheStatus()).toBe('MISS');
  });

  test('returns HIT for a non-expired cached response', async () => {
    const payload = { cep: '01310-100' };
    global.fetch.mockResolvedValue(okJson(payload));

    await cachedFetch('https://example.test/cep');
    await expect(cachedFetch('https://example.test/cep')).resolves.toEqual(payload);

    expect(global.fetch).toHaveBeenCalledTimes(1);
    expect(cacheSize()).toBe(1);
    expect(getLastCacheStatus()).toBe('HIT');
  });

  test('refetches after the success TTL expires', async () => {
    jest.spyOn(Date, 'now')
      .mockReturnValueOnce(1_000)
      .mockReturnValueOnce(1_005)
      .mockReturnValueOnce(1_011)
      .mockReturnValueOnce(1_011);

    global.fetch
      .mockResolvedValueOnce(okJson({ version: 1 }))
      .mockResolvedValueOnce(okJson({ version: 2 }));

    await expect(
      cachedFetch('https://example.test/cep', { successTtl: 10 })
    ).resolves.toEqual({ version: 1 });

    await expect(
      cachedFetch('https://example.test/cep', { successTtl: 10 })
    ).resolves.toEqual({ version: 1 });

    await expect(
      cachedFetch('https://example.test/cep', { successTtl: 10 })
    ).resolves.toEqual({ version: 2 });

    expect(global.fetch).toHaveBeenCalledTimes(2);
    expect(getLastCacheStatus()).toBe('MISS');
  });

  test('uses notFoundTtl for ViaCEP not-found payloads', async () => {
    jest.spyOn(Date, 'now')
      .mockReturnValueOnce(2_000)
      .mockReturnValueOnce(2_004)
      .mockReturnValueOnce(2_006)
      .mockReturnValueOnce(2_006);

    global.fetch
      .mockResolvedValueOnce(okJson({ erro: true }))
      .mockResolvedValueOnce(okJson({ erro: true, retry: true }));

    await expect(
      cachedFetch('https://example.test/missing', { notFoundTtl: 5 })
    ).resolves.toEqual({ erro: true });

    await expect(
      cachedFetch('https://example.test/missing', { notFoundTtl: 5 })
    ).resolves.toEqual({ erro: true });

    await expect(
      cachedFetch('https://example.test/missing', { notFoundTtl: 5 })
    ).resolves.toEqual({ erro: true, retry: true });

    expect(global.fetch).toHaveBeenCalledTimes(2);
    expect(getLastCacheStatus()).toBe('MISS');
  });

  test('keeps separate cache entries for different URLs', async () => {
    global.fetch
      .mockResolvedValueOnce(okJson({ cep: '01310-100' }))
      .mockResolvedValueOnce(okJson({ cep: '22041-001' }));

    await cachedFetch('https://example.test/01310100');
    await cachedFetch('https://example.test/22041001');

    expect(global.fetch).toHaveBeenCalledTimes(2);
    expect(cacheSize()).toBe(2);
  });

  test('does not cache HTTP errors', async () => {
    global.fetch.mockResolvedValue({ ok: false });

    await expect(cachedFetch('https://example.test/error')).rejects.toThrow(
      /consultar o CEP agora/
    );

    expect(global.fetch).toHaveBeenCalledTimes(1);
    expect(cacheSize()).toBe(0);
    expect(getLastCacheStatus()).toBe('MISS');
  });

  test('does not cache fetch rejections', async () => {
    global.fetch.mockRejectedValue(new Error('network down'));

    await expect(cachedFetch('https://example.test/network')).rejects.toThrow(
      /consultar o CEP agora/
    );

    expect(global.fetch).toHaveBeenCalledTimes(1);
    expect(cacheSize()).toBe(0);
    expect(getLastCacheStatus()).toBe('MISS');
  });

  test('does not cache invalid JSON responses', async () => {
    global.fetch.mockResolvedValue({
      ok: true,
      json: jest.fn().mockRejectedValue(new Error('invalid json')),
    });

    await expect(cachedFetch('https://example.test/json')).rejects.toThrow(
      /consultar o CEP agora/
    );

    expect(global.fetch).toHaveBeenCalledTimes(1);
    expect(cacheSize()).toBe(0);
    expect(getLastCacheStatus()).toBe('MISS');
  });

  test('flushCache clears entries and resets the last cache status', async () => {
    global.fetch.mockResolvedValue(okJson({ cep: '01310-100' }));

    await cachedFetch('https://example.test/cep');

    expect(cacheSize()).toBe(1);
    expect(getLastCacheStatus()).toBe('MISS');

    flushCache();

    expect(cacheSize()).toBe(0);
    expect(getLastCacheStatus()).toBeNull();
  });
});
