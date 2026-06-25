import {
  persistAuthResponse,
  loginWithEmail,
  logout,
  registerWithEmail,
  deleteUserAccount,
  loginWithGoogle,
} from '../services/authService';
import { saveAuthData, clearAuthStorage } from '../services/tokenStorageService';
import { apiFetch, API_BASE } from '../services/apiClient';

jest.mock('../services/tokenStorageService', () => ({
  saveAuthData: jest.fn(),
  clearAuthStorage: jest.fn(),
}));

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
  API_BASE: 'http://api.test',
}));

function okJsonResponse(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('authService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    global.fetch = jest.fn();
    saveAuthData.mockResolvedValue(true);
  });

  describe('persistAuthResponse', () => {
    test('normalizes a PascalCase response and persists it', async () => {
      const auth = await persistAuthResponse({
        UsuarioId: '1',
        Nome: 'Ana',
        Email: 'ana@example.com',
        Token: 'jwt-token',
      });

      expect(auth).toEqual({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com', token: 'jwt-token' });
      expect(saveAuthData).toHaveBeenCalledWith(auth);
    });

    test('normalizes a camelCase response and persists it', async () => {
      const auth = await persistAuthResponse({
        usuarioId: '2',
        nome: 'Bia',
        email: 'bia@example.com',
        token: 'jwt-token-2',
      });

      expect(auth).toEqual({ usuarioId: '2', nome: 'Bia', email: 'bia@example.com', token: 'jwt-token-2' });
    });

    test('throws when there is no token in the response', async () => {
      await expect(persistAuthResponse({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com' })).rejects.toThrow(
        'Resposta de autenticação sem token.'
      );

      expect(saveAuthData).not.toHaveBeenCalled();
    });
  });

  describe('loginWithEmail', () => {
    test('posts to the login endpoint with the mapped payload', async () => {
      global.fetch.mockResolvedValue(
        okJsonResponse({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com', token: 'jwt-token' })
      );

      await loginWithEmail({ email: 'ana@example.com', password: 'Senha@123' });

      expect(global.fetch).toHaveBeenCalledWith(
        `${API_BASE}/api/auth/login`,
        expect.objectContaining({
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ Email: 'ana@example.com', Senha: 'Senha@123' }),
        })
      );
    });

    test('returns the normalized user and token on success', async () => {
      global.fetch.mockResolvedValue(
        okJsonResponse({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com', token: 'jwt-token' })
      );

      const result = await loginWithEmail({ email: 'ana@example.com', password: 'Senha@123' });

      expect(result).toEqual({
        user: { id: '1', name: 'Ana', email: 'ana@example.com' },
        token: 'jwt-token',
      });
    });

    test('throws a descriptive error when the request fails', async () => {
      global.fetch.mockResolvedValue({
        ok: false,
        status: 401,
        text: jest.fn().mockResolvedValue('Credenciais inválidas'),
      });

      await expect(loginWithEmail({ email: 'ana@example.com', password: 'wrong' })).rejects.toThrow(
        'Autenticação falhou: 401 Credenciais inválidas'
      );
    });
  });

  describe('logout', () => {
    test('clears the auth storage', async () => {
      await logout();

      expect(clearAuthStorage).toHaveBeenCalledTimes(1);
    });
  });

  describe('registerWithEmail', () => {
    const baseRegisterInput = {
      name: 'Ana',
      email: 'ana@example.com',
      password: 'Senha@123',
      birthDate: '15/06/2000',
      cep: '01310-100',
      street: 'Avenida Paulista',
      number: '100',
      complement: '',
      neighborhood: 'Bela Vista',
      city: 'São Paulo',
      consentimentoAceito: true,
    };

    test('posts to the register endpoint with the mapped payload, computed age and ISO birth date', async () => {
      global.fetch.mockResolvedValue(
        okJsonResponse({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com', token: 'jwt-token' })
      );

      await registerWithEmail(baseRegisterInput);

      const [, options] = global.fetch.mock.calls[0];
      const body = JSON.parse(options.body);

      expect(body).toMatchObject({
        Nome: 'Ana',
        Email: 'ana@example.com',
        Senha: 'Senha@123',
        DataNascimento: '2000-06-15',
        Cep: '01310-100',
        Logradouro: 'Avenida Paulista',
        Numero: '100',
        Complemento: null,
        Bairro: 'Bela Vista',
        Cidade: 'São Paulo',
        ConsentimentoAceito: true,
      });
      expect(typeof body.Idade).toBe('number');
    });

    test('keeps a non-empty complement instead of converting it to null', async () => {
      global.fetch.mockResolvedValue(
        okJsonResponse({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com', token: 'jwt-token' })
      );

      await registerWithEmail({ ...baseRegisterInput, complement: 'Apto 12' });

      const [, options] = global.fetch.mock.calls[0];
      const body = JSON.parse(options.body);

      expect(body.Complemento).toBe('Apto 12');
    });

    test('sends a null DataNascimento when birthDate is empty or malformed', async () => {
      global.fetch.mockResolvedValue(
        okJsonResponse({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com', token: 'jwt-token' })
      );

      await registerWithEmail({ ...baseRegisterInput, birthDate: '' });

      const [, options] = global.fetch.mock.calls[0];
      const body = JSON.parse(options.body);

      expect(body.DataNascimento).toBeNull();
    });

    test('returns the normalized user and token on success', async () => {
      global.fetch.mockResolvedValue(
        okJsonResponse({ usuarioId: '1', nome: 'Ana', email: 'ana@example.com', token: 'jwt-token' })
      );

      const result = await registerWithEmail(baseRegisterInput);

      expect(result).toEqual({
        user: { id: '1', name: 'Ana', email: 'ana@example.com' },
        token: 'jwt-token',
      });
    });

    test('throws a descriptive error when the request fails', async () => {
      global.fetch.mockResolvedValue({
        ok: false,
        status: 409,
        text: jest.fn().mockResolvedValue('E-mail já cadastrado.'),
      });

      await expect(registerWithEmail(baseRegisterInput)).rejects.toThrow(
        'Autenticação falhou: 409 E-mail já cadastrado.'
      );
    });
  });

  describe('deleteUserAccount', () => {
    test('calls apiFetch with DELETE on the user endpoint', async () => {
      apiFetch.mockResolvedValue({ ok: true });

      await deleteUserAccount('user-1');

      expect(apiFetch).toHaveBeenCalledWith('/api/Usuario/user-1', { method: 'DELETE' });
    });

    test('throws using the problem detail message when the request fails', async () => {
      apiFetch.mockResolvedValue({
        ok: false,
        status: 404,
        json: jest.fn().mockResolvedValue({ detail: 'Usuário não encontrado.' }),
      });

      await expect(deleteUserAccount('user-1')).rejects.toThrow('Usuário não encontrado.');
    });

    test('falls back to the title when there is no detail in the error body', async () => {
      apiFetch.mockResolvedValue({
        ok: false,
        status: 404,
        json: jest.fn().mockResolvedValue({ title: 'Recurso não encontrado' }),
      });

      await expect(deleteUserAccount('user-1')).rejects.toThrow('Recurso não encontrado');
    });

    test('falls back to a generic error message when the error body has neither detail nor title', async () => {
      apiFetch.mockResolvedValue({
        ok: false,
        status: 500,
        json: jest.fn().mockResolvedValue({}),
      });

      await expect(deleteUserAccount('user-1')).rejects.toThrow('Erro 500');
    });

    test('falls back to a generic error message when the error body cannot be parsed as JSON', async () => {
      apiFetch.mockResolvedValue({
        ok: false,
        status: 500,
        json: jest.fn().mockRejectedValue(new Error('invalid json')),
      });

      await expect(deleteUserAccount('user-1')).rejects.toThrow('Erro 500');
    });
  });

  describe('loginWithGoogle', () => {
    test('throws a not-implemented error', async () => {
      await expect(loginWithGoogle()).rejects.toThrow('Login com Google ainda não implementado.');
    });
  });
});