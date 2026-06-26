import { renderHook, act, waitFor } from '@testing-library/react';

import useEditProfileViewModel from '../viewmodels/useEditProfileViewModel';
import { getProfileData, updateProfileData } from '../repositories/profileRepository';

jest.mock('../repositories/profileRepository', () => ({
  getProfileData: jest.fn(),
  updateProfileData: jest.fn(),
}));

const VALID_VALUES = {
  name: 'Ana Silva',
  email: 'ana@example.com',
  birthDate: '15/06/2000',
  cep: '01310-100',
  street: 'Avenida Paulista',
  number: '100',
  complement: '',
  neighborhood: 'Bela Vista',
  city: 'São Paulo',
};

describe('useEditProfileViewModel', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.spyOn(console, 'warn').mockImplementation(() => {});
    getProfileData.mockResolvedValue({ userProfile: {} });
  });

  afterEach(() => {
    console.warn.mockRestore();
  });

  describe('initial load', () => {
    test('starts in a loading state and finishes loading once the profile resolves', async () => {
      getProfileData.mockResolvedValue({ userProfile: { name: 'Ana' } });

      const { result } = renderHook(() => useEditProfileViewModel());

      expect(result.current.loading).toBe(true);

      await waitFor(() => expect(result.current.loading).toBe(false));
    });

    test('populates the form values from the loaded profile', async () => {
      getProfileData.mockResolvedValue({ userProfile: VALID_VALUES });

      const { result } = renderHook(() => useEditProfileViewModel());

      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(result.current.values).toEqual(VALID_VALUES);
    });

    test('falls back to empty strings for any missing profile field', async () => {
      getProfileData.mockResolvedValue({ userProfile: { name: 'Ana' } });

      const { result } = renderHook(() => useEditProfileViewModel());

      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(result.current.values.email).toBe('');
      expect(result.current.values.cep).toBe('');
    });

    test('falls back to empty values when userProfile itself is missing', async () => {
      getProfileData.mockResolvedValue({});

      const { result } = renderHook(() => useEditProfileViewModel());

      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(result.current.values.name).toBe('');
    });

    test('logs a warning and stops loading when getProfileData rejects', async () => {
      getProfileData.mockRejectedValue(new Error('network down'));

      const { result } = renderHook(() => useEditProfileViewModel());

      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(console.warn).toHaveBeenCalled();
    });
  });

  describe('updateField', () => {
    test('updates the given field and clears its previous error', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));

      act(() => {
        result.current.updateField('name', '');
      });
      await act(async () => {
        await result.current.saveProfile();
      });
      expect(result.current.errors.name).toBeDefined();

      act(() => {
        result.current.updateField('name', 'Ana Silva');
      });

      expect(result.current.values.name).toBe('Ana Silva');
      expect(result.current.errors.name).toBeUndefined();
    });
  });

  describe('saveProfile validation', () => {
    async function setValues(result, values) {
      act(() => {
        Object.entries(values).forEach(([field, value]) => {
          result.current.updateField(field, value);
        });
      });
    }

    test('rejects when name is empty', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, name: '   ' });

      let outcome;
      await act(async () => {
        outcome = await result.current.saveProfile();
      });

      expect(outcome).toEqual({ ok: false, reason: 'validation' });
      expect(result.current.errors.name).toBe('Informe seu nome.');
      expect(updateProfileData).not.toHaveBeenCalled();
    });

    test('rejects when email is empty', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, email: '' });

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.errors.email).toBe('Informe seu e-mail.');
    });

    test('rejects when email has no @', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, email: 'ana-sem-arroba.com' });

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.errors.email).toBe('Informe um e-mail válido.');
    });

    test('accepts an empty birthDate (optional field)', async () => {
      updateProfileData.mockResolvedValue({});
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, birthDate: '' });

      let outcome;
      await act(async () => {
        outcome = await result.current.saveProfile();
      });

      expect(outcome).toEqual({ ok: true });
      expect(result.current.errors.birthDate).toBeUndefined();
    });

    test('rejects a birthDate with an invalid calendar date', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, birthDate: '31/02/2000' });

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.errors.birthDate).toBe(
        'Informe uma data válida no formato DD/MM/AAAA.'
      );
    });

    test('accepts a valid birthDate', async () => {
      updateProfileData.mockResolvedValue({});
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, VALID_VALUES);

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.errors.birthDate).toBeUndefined();
    });

    test('rejects when cep is empty', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, cep: '' });

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.errors.cep).toBe('Informe seu CEP.');
    });

    test('rejects when cep does not have 8 digits', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, cep: '123' });

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.errors.cep).toBe('Informe um CEP válido com 8 dígitos.');
    });

    test('rejects when street, number, neighborhood or city are empty', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, {
        ...VALID_VALUES,
        street: '',
        number: '',
        neighborhood: '',
        city: '',
      });

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.errors.street).toBe('Informe sua rua.');
      expect(result.current.errors.number).toBe('Informe o número.');
      expect(result.current.errors.neighborhood).toBe('Informe seu bairro.');
      expect(result.current.errors.city).toBe('Informe sua cidade.');
    });

    test('does not set saving to true when validation fails', async () => {
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));
      await setValues(result, { ...VALID_VALUES, name: '' });

      await act(async () => {
        await result.current.saveProfile();
      });

      expect(result.current.saving).toBe(false);
    });
  });

  describe('saveProfile success path', () => {
    test('calls updateProfileData with the current values and returns ok: true', async () => {
      updateProfileData.mockResolvedValue({});
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));

      act(() => {
        Object.entries(VALID_VALUES).forEach(([field, value]) => {
          result.current.updateField(field, value);
        });
      });

      let outcome;
      await act(async () => {
        outcome = await result.current.saveProfile();
      });

      expect(updateProfileData).toHaveBeenCalledWith(expect.objectContaining({ name: 'Ana Silva' }));
      expect(outcome).toEqual({ ok: true });
    });

    test('resets saving back to false even when updateProfileData throws', async () => {
      updateProfileData.mockRejectedValue(new Error('server error'));
      const { result } = renderHook(() => useEditProfileViewModel());
      await waitFor(() => expect(result.current.loading).toBe(false));

      act(() => {
        Object.entries(VALID_VALUES).forEach(([field, value]) => {
          result.current.updateField(field, value);
        });
      });

      await act(async () => {
        await expect(result.current.saveProfile()).rejects.toThrow('server error');
      });

      expect(result.current.saving).toBe(false);
    });
  });
});