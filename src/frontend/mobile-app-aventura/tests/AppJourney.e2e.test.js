jest.mock('../services/tokenStorageService', () => {
  let token = null;
  let session = null;


  return {
    saveAuthData: jest.fn(async (auth) => {
      token = auth.token;
      session = {
        usuarioId: auth.usuarioId,
        nome: auth.nome,
        email: auth.email,
      };
      return true;
    }),
    saveAuthToken: jest.fn(async (value) => {
      token = value;
      return true;
    }),
    getAuthToken: jest.fn(async () => token),
    saveAuthSession: jest.fn(async (value) => {
      session = value;
      return true;
    }),
    getAuthSession: jest.fn(async () => session),
    clearAuthStorage: jest.fn(async () => {
      token = null;
      session = null;
      return true;
    }),
  };
});


jest.mock('../navigation/navigationRef', () => ({
  handleSessionExpired: jest.fn(),
}));


const mockAsyncStorage = {};


jest.mock('@react-native-async-storage/async-storage', () => ({
  getItem: jest.fn((key) => Promise.resolve(mockAsyncStorage[key] ?? null)),
  setItem: jest.fn((key, value) => {
    mockAsyncStorage[key] = value;
    return Promise.resolve();
  }),
  removeItem: jest.fn((key) => {
    delete mockAsyncStorage[key];
    return Promise.resolve();
  }),
  multiRemove: jest.fn((keys) => {
    keys.forEach((key) => delete mockAsyncStorage[key]);
    return Promise.resolve();
  }),
}));


import { registerWithEmail, loginWithEmail, deleteUserAccount } from '../services/authService';
import { fetchCategories } from '../services/categoryService';
import { onboardingService } from '../services/onboardingService';
import { fetchPreferences } from '../services/preferenceService';
import { fetchRecommendations } from '../services/recommendationService';
import { fetchShows, fetchShowById, mapDtoToShowInfo, searchShows } from '../services/showService';
import { fetchTheaterShows, fetchTheaters } from '../services/theaterService';
import { getUserInfo, updateUserInfo } from '../services/userProfileService';
import { lookupAddressByCep } from '../services/viaCepService';
import { CACHE_STATUS, flushCache, getLastCacheStatus } from '../services/vhsService';
import { clearAuthStorage } from '../services/tokenStorageService';
import { handleSessionExpired } from '../navigation/navigationRef';
import {
  addEventToWishlist,
  getStoredWishlist,
  isEventInWishlist,
  removeEventFromWishlist,
  toggleEventInWishlist,
} from '../services/wishlistStorageService';


function jsonResponse(body, status = 200) {
  return {
    ok: status >= 200 && status < 300,
    status,
    json: jest.fn().mockResolvedValue(body),
    text: jest.fn().mockResolvedValue(typeof body === 'string' ? body : JSON.stringify(body)),
  };
}


function createApiState() {
  const show = {
    Id: 'show-1',
    Nome: 'A Noviça Rebelde',
    Genero: 'Musical',
    TipoEvento: 'Musical',
    Descricao: 'Clássico para toda a família.',
    UrlBilheteria: 'https://ticketera.com/novica',
    DataInicio: '2026-07-01T19:00:00Z',
    DataFim: '2026-08-01T19:00:00Z',
    ValorIngresso: 120,
    EmCartaz: true,
  };


  return {
    token: 'jwt-e2e-token',
    user: {
      UsuarioId: 'user-1',
      Nome: 'Ana Luiza',
      Email: 'ana.e2e@aventura.test',
      Token: 'jwt-e2e-token',
    },
    profile: {
      Id: 'user-1',
      Nome: 'Ana Luiza',
      Email: 'ana.e2e@aventura.test',
      Idade: 29,
      Cep: '01310-100',
      Logradouro: 'Avenida Paulista',
      Numero: '1000',
      Bairro: 'Bela Vista',
      Cidade: 'São Paulo',
    },
    preferences: {
      UsuarioId: 'user-1',
      Cidade: 'São Paulo',
      Cep: '01310-100',
      Generos: ['Musical'],
      Tags: ['monthly'],
      ReceberNotificacoes: true,
    },
    categories: ['Musical', 'Stand-Up', 'Drama'],
    shows: [show],
    theaters: [
      {
        Id: 'theater-1',
        Nome: 'Teatro Renault',
        Endereco: 'Av. Brigadeiro Luís Antônio, 411',
        Cidade: 'São Paulo',
        HorarioFuncionamento: '10h às 22h',
      },
    ],
    recommendations: [
      {
        Espetaculo: show,
        Score: 0.96,
        Motivo: 'Baseado nos seus gêneros favoritos',
      },
    ],
    deleted: false,
  };
}


function installFetchApi(state) {
  global.fetch = jest.fn(async (url, options = {}) => {
    const path = String(url).replace('http://163.176.204.208:5253', '');
    const method = options.method ?? 'GET';
    const headers = options.headers ?? {};


    if (String(url).startsWith('https://viacep.com.br/ws/01310100/json/')) {
      return jsonResponse({
        cep: '01310-100',
        logradouro: 'Avenida Paulista',
        bairro: 'Bela Vista',
        localidade: 'São Paulo',
        uf: 'SP',
      });
    }


    const isAuthEndpoint = path.startsWith('/api/auth/');
    const authorization = headers.Authorization ?? headers.authorization;


    if (!isAuthEndpoint && authorization !== `Bearer ${state.token}`) {
      return jsonResponse({ title: 'Não autorizado' }, 401);
    }


    if (method === 'POST' && path === '/api/auth/register') {
      return jsonResponse(state.user, 201);
    }


    if (method === 'POST' && path === '/api/auth/login') {
      return state.deleted ? jsonResponse({ title: 'Conta encerrada' }, 401) : jsonResponse(state.user);
    }


    if (method === 'GET' && path === '/api/Usuario/user-1') {
      return jsonResponse(state.profile);
    }


    if (method === 'PUT' && path === '/api/Usuario/user-1') {
      const body = JSON.parse(options.body);
      state.profile = {
        ...state.profile,
        ...body,
        Nome: body.Nome ?? state.profile.Nome,
        Email: body.Email ?? state.profile.Email,
        Idade: body.Idade ?? state.profile.Idade,
        Cep: body.Cep ?? state.profile.Cep,
        Logradouro: body.Logradouro ?? state.profile.Logradouro,
        Numero: body.Numero ?? state.profile.Numero,
        Complemento: body.Complemento ?? state.profile.Complemento,
        Bairro: body.Bairro ?? state.profile.Bairro,
        Cidade: body.Cidade ?? state.profile.Cidade,
      };
      return jsonResponse(state.profile);
    }


    if (method === 'DELETE' && path === '/api/Usuario/user-1') {
      state.deleted = true;
      return jsonResponse(null, 204);
    }


    if (method === 'GET' && path === '/api/Genero') {
      return jsonResponse(state.categories);
    }


    if (method === 'PUT' && path === '/api/Preferencias/user-1') {
      const body = JSON.parse(options.body);
      state.preferences = {
        ...state.preferences,
        Cidade: body.Cidade,
        Generos: body.Generos,
        Tags: body.Tags,
        ReceberNotificacoes: body.ReceberNotificacoes,
      };
      return jsonResponse(state.preferences);
    }


    if (method === 'GET' && path === '/api/Preferencias/user-1') {
      return jsonResponse(state.preferences);
    }


    if (method === 'GET' && path === '/api/Espetaculo') {
      return jsonResponse(state.shows);
    }


    if (method === 'GET' && path.startsWith('/api/Espetaculo/search')) {
      return jsonResponse(state.shows);
    }


    if (method === 'GET' && path === '/api/Espetaculo/show-1') {
      return jsonResponse(state.shows[0]);
    }


    if (method === 'GET' && path === '/api/Teatro') {
      return jsonResponse(state.theaters);
    }


    if (method === 'GET' && path === '/api/Teatro/theater-1/espetaculos') {
      return jsonResponse(state.shows);
    }


    if (method === 'GET' && path === '/api/Recomendacao/user-1?limit=5') {
      return jsonResponse(state.recommendations);
    }


    return jsonResponse({ title: `Rota não mapeada no E2E: ${method} ${path}` }, 404);
  });
}


describe('E2E mobile app journey', () => {
  beforeEach(async () => {
    jest.clearAllMocks();
    Object.keys(mockAsyncStorage).forEach((key) => delete mockAsyncStorage[key]);
    flushCache();
    await clearAuthStorage();
  });


  test('covers cadastro, ViaCEP, onboarding, descoberta, recomendações, bilheteria e encerramento de conta', async () => {
    const state = createApiState();
    installFetchApi(state);


    const address = await lookupAddressByCep('01310-100');
    expect(address).toMatchObject({
      cep: '01310-100',
      street: 'Avenida Paulista',
      city: 'São Paulo',
    });
    expect(getLastCacheStatus()).toBe(CACHE_STATUS.MISS);


    await lookupAddressByCep('01310-100');
    expect(getLastCacheStatus()).toBe(CACHE_STATUS.HIT);


    const register = await registerWithEmail({
      name: 'Ana Luiza',
      email: 'ana.e2e@aventura.test',
      password: 'Senha@123',
      birthDate: '15/06/1997',
      cep: address.cep,
      street: address.street,
      number: '1000',
      complement: '',
      neighborhood: address.neighborhood,
      city: address.city,
      consentimentoAceito: true,
    });


    expect(register.user).toEqual({
      id: 'user-1',
      name: 'Ana Luiza',
      email: 'ana.e2e@aventura.test',
    });


    const profile = await getUserInfo();
    expect(profile).toMatchObject({
      id: 'user-1',
      name: 'Ana Luiza',
      city: 'São Paulo',
      cep: '01310-100',
    });


    const updatedProfile = await updateUserInfo({
      Nome: 'Ana Luiza Atualizada',
      Cidade: 'Rio de Janeiro',
      Cep: '22040-002',
      Logradouro: 'Avenida Atlântica',
      Numero: '1000',
      Bairro: 'Copacabana',
    });
    expect(updatedProfile).toMatchObject({
      name: 'Ana Luiza Atualizada',
      city: 'Rio de Janeiro',
      cep: '22040-002',
    });


    const categories = await fetchCategories();
    expect(categories.map((category) => category.label)).toEqual(['Todos', 'Musical', 'Stand-Up', 'Drama']);


    const savedOnboarding = await onboardingService.save({
      categories: ['0ac88c96-1f1d-48dd-bc9d-9b08c00bdc65'],
      city: 'sao_paulo',
      frequency: 'todo_mes',
    });
    expect(savedOnboarding.Generos).toContain('Musical');


    const preferences = await fetchPreferences('user-1');
    expect(preferences.Cidade).toBe('São Paulo');


    const shows = await fetchShows();
    expect(shows).toHaveLength(1);


    const search = await searchShows('Noviça', 'Musical');
    expect(search[0].Nome).toBe('A Noviça Rebelde');


    const details = mapDtoToShowInfo(await fetchShowById('show-1'));
    expect(details.ticket.url).toBe('https://ticketera.com/novica');
    expect(details.ticket.price).toBe('R$ 120,00');


    await addEventToWishlist({ id: details.id, title: details.title });
    await addEventToWishlist({ id: details.id, title: details.title });
    expect(await getStoredWishlist()).toEqual([{ id: 'show-1', title: 'A Noviça Rebelde' }]);
    expect(await isEventInWishlist('show-1')).toBe(true);


    await toggleEventInWishlist({ id: details.id, title: details.title });
    expect(await isEventInWishlist('show-1')).toBe(false);


    await addEventToWishlist({ id: details.id, title: details.title });
    await removeEventFromWishlist('show-1');
    expect(await getStoredWishlist()).toEqual([]);


    const theaters = await fetchTheaters();
    expect(theaters[0].Nome).toBe('Teatro Renault');


    const theaterShows = await fetchTheaterShows('theater-1');
    expect(theaterShows[0].Id).toBe('show-1');


    const recommendations = await fetchRecommendations('user-1');
    expect(recommendations[0]).toMatchObject({
      id: 'show-1',
      title: 'A Noviça Rebelde',
      recommendationScore: 0.96,
      recommendationReason: 'Baseado nos seus gêneros favoritos',
    });


    await deleteUserAccount('user-1');
    await expect(loginWithEmail({ email: 'ana.e2e@aventura.test', password: 'Senha@123' })).rejects.toThrow(
      'Autenticação falhou: 401'
    );
  });


  test('expires the mobile session when a protected endpoint returns 401', async () => {
    const state = createApiState();
    installFetchApi({ ...state, token: 'different-token' });


    await registerWithEmail({
      name: 'Ana Luiza',
      email: 'ana.e2e@aventura.test',
      password: 'Senha@123',
      birthDate: '15/06/1997',
      cep: '01310-100',
      street: 'Avenida Paulista',
      number: '1000',
      complement: '',
      neighborhood: 'Bela Vista',
      city: 'São Paulo',
      consentimentoAceito: true,
    });


    await expect(fetchShows()).rejects.toThrow('Nao foi possivel carregar os espetaculos');
    expect(handleSessionExpired).toHaveBeenCalledTimes(1);
  });
});

