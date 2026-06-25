import { getUserInfo, updateUserInfo } from '../services/userProfileService';
import { apiFetch } from '../services/apiClient';
import { getAuthSession } from '../services/tokenStorageService';

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
}));

jest.mock('../services/tokenStorageService', () => ({
  getAuthSession: jest.fn(),
}));

function okJson(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('userProfileService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.spyOn(console, 'error').mockImplementation(() => {});
  });

  afterEach(() => {
    console.error.mockRestore();
  });

  describe('getUserInfo', () => {
    test('throws when there is no active session', async () => {
      getAuthSession.mockResolvedValue(null);

      await expect(getUserInfo()).rejects.toThrow('Sessão de usuário não encontrada.');
      expect(apiFetch).not.toHaveBeenCalled();
    });

    test('fetches the user by id from the session and normalizes PascalCase fields', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue(
        okJson({
          Id: 'user-1',
          Nome: 'Ana',
          Email: 'ana@example.com',
          Idade: 25,
          DataNascimento: '2000-06-15T00:00:00',
          Cep: '01310-100',
          Logradouro: 'Avenida Paulista',
          Numero: '100',
          Complemento: 'Apto 1',
          Bairro: 'Bela Vista',
          Cidade: 'São Paulo',
        })
      );

      const profile = await getUserInfo();

      expect(apiFetch).toHaveBeenCalledWith('/api/Usuario/user-1');
      expect(profile).toEqual({
        id: 'user-1',
        name: 'Ana',
        email: 'ana@example.com',
        age: 25,
        birthDate: '15/06/2000',
        cep: '01310-100',
        street: 'Avenida Paulista',
        number: '100',
        complement: 'Apto 1',
        neighborhood: 'Bela Vista',
        city: 'São Paulo',
      });
    });

    test('normalizes camelCase fields as a fallback', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue(okJson({ id: 'user-1', nome: 'Bia', email: 'bia@example.com' }));

      const profile = await getUserInfo();

      expect(profile.id).toBe('user-1');
      expect(profile.name).toBe('Bia');
      expect(profile.email).toBe('bia@example.com');
      expect(profile.age).toBeNull();
      expect(profile.birthDate).toBe('');
    });

    test('returns empty strings for missing optional address fields', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue(okJson({ id: 'user-1', nome: 'Bia', email: 'bia@example.com' }));

      const profile = await getUserInfo();

      expect(profile.cep).toBe('');
      expect(profile.street).toBe('');
      expect(profile.number).toBe('');
      expect(profile.complement).toBe('');
      expect(profile.neighborhood).toBe('');
      expect(profile.city).toBe('');
    });

    test('throws with the response status and body when the request fails', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue({ ok: false, status: 404, text: jest.fn().mockResolvedValue('Não encontrado') });

      await expect(getUserInfo()).rejects.toThrow('Erro 404: Não encontrado');
    });
    test('returns an empty birthDate when dataNascimento is present but malformed', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue(
        okJson({ id: 'user-1', nome: 'Bia', email: 'bia@example.com', dataNascimento: '2000-06' })
      );

      const profile = await getUserInfo();

      expect(profile.birthDate).toBe('');
    });
  });

  describe('updateUserInfo', () => {
    test('throws when there is no active session', async () => {
      getAuthSession.mockResolvedValue(null);

      await expect(updateUserInfo({ name: 'Ana' })).rejects.toThrow('Sessão de usuário não encontrada.');
      expect(apiFetch).not.toHaveBeenCalled();
    });

    test('sends a PUT with the JSON payload to the user endpoint', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue(okJson({ id: 'user-1', nome: 'Ana Atualizada', email: 'ana@example.com' }));

      await updateUserInfo({ Nome: 'Ana Atualizada' });

      expect(apiFetch).toHaveBeenCalledWith('/api/Usuario/user-1', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Nome: 'Ana Atualizada' }),
      });
    });

    test('returns the normalized updated profile on success', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue(okJson({ id: 'user-1', nome: 'Ana Atualizada', email: 'ana@example.com' }));

      const profile = await updateUserInfo({ Nome: 'Ana Atualizada' });

      expect(profile.name).toBe('Ana Atualizada');
    });

    test('throws with the response status and body when the request fails', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      apiFetch.mockResolvedValue({
        ok: false,
        status: 400,
        text: jest.fn().mockResolvedValue('CEP inválido'),
      });

      await expect(updateUserInfo({ Cep: '00000-000' })).rejects.toThrow(
        'Erro ao atualizar usuário: 400 CEP inválido'
      );
    });
  });
});