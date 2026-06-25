import { renderHook, act, waitFor } from '@testing-library/react';

import useProfileViewModel from '../viewmodels/useProfileViewModel';
import { removeUserAccount, signOut } from '../repositories/authRepository';
import { getProfileData } from '../repositories/profileRepository';
import { clearCurrentUserId } from '../services/userSession';
import { getAuthSession } from '../services/tokenStorageService';

jest.mock('../repositories/authRepository', () => ({
  removeUserAccount: jest.fn(),
  signOut: jest.fn(),
}));

jest.mock('../repositories/profileRepository', () => ({
  getProfileData: jest.fn(),
}));

jest.mock('../services/userSession', () => ({
  clearCurrentUserId: jest.fn(),
}));

jest.mock('../services/tokenStorageService', () => ({
  getAuthSession: jest.fn(),
}));

function createNavigation() {
  return {
    navigate: jest.fn(),
    reset: jest.fn(),
  };
}

describe('useProfileViewModel', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.spyOn(console, 'log').mockImplementation(() => {});
    jest.spyOn(console, 'warn').mockImplementation(() => {});
    getProfileData.mockResolvedValue({
      userProfile: { name: 'Ana Silva', email: 'ana@example.com' },
      profileMenuItems: [{ id: 'preferences', title: 'Preferências' }],
    });
  });

  afterEach(() => {
    console.log.mockRestore();
    console.warn.mockRestore();
  });

  describe('initial load', () => {
    test('loads the profile and menu items on mount', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));

      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      expect(result.current.user).toEqual({ name: 'Ana Silva', email: 'ana@example.com' });
      expect(result.current.menuItems).toEqual([{ id: 'preferences', title: 'Preferências' }]);
    });

    test('falls back to empty user and menu items when the repository returns nothing', async () => {
      getProfileData.mockResolvedValue({});
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));

      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      expect(result.current.user).toEqual({});
      expect(result.current.menuItems).toEqual([]);
    });

    test('resets to empty state and warns when getProfileData rejects', async () => {
      getProfileData.mockRejectedValue(new Error('network down'));
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));

      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      expect(result.current.user).toEqual({});
      expect(result.current.menuItems).toEqual([]);
      expect(console.warn).toHaveBeenCalled();
    });
  });

  describe('refreshProfile', () => {
    test('can be called manually to reload the profile data', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));
      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      getProfileData.mockResolvedValue({
        userProfile: { name: 'Ana Atualizada' },
        profileMenuItems: [],
      });

      await act(async () => {
        await result.current.refreshProfile();
      });

      expect(result.current.user).toEqual({ name: 'Ana Atualizada' });
    });
  });

  describe('handleMenuPress', () => {
    test('logs the pressed menu item title', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));
      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      result.current.handleMenuPress({ title: 'Preferências' });

      expect(console.log).toHaveBeenCalledWith('Abrir item:', 'Preferências');
    });
  });

  describe('handleEditProfile', () => {
    test('navigates to the EditProfile screen', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));
      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      result.current.handleEditProfile();

      expect(navigation.navigate).toHaveBeenCalledWith('EditProfile');
    });
  });

  describe('handleLogout', () => {
    test('signs out, clears the current user id and resets navigation to Auth', async () => {
      signOut.mockResolvedValue(undefined);
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));
      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      await act(async () => {
        await result.current.handleLogout();
      });

      expect(signOut).toHaveBeenCalledTimes(1);
      expect(clearCurrentUserId).toHaveBeenCalledTimes(1);
      expect(navigation.reset).toHaveBeenCalledWith({
        index: 0,
        routes: [{ name: 'Auth' }],
      });
    });
  });

  describe('handleDeleteAccount', () => {
    test('throws when there is no active session', async () => {
      getAuthSession.mockResolvedValue(null);
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));
      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      await expect(result.current.handleDeleteAccount()).rejects.toThrow(
        'Sessao invalida. Faca login novamente.'
      );
      expect(removeUserAccount).not.toHaveBeenCalled();
    });

    test('sets deletingAccount while removing the account and signs out on success', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      removeUserAccount.mockResolvedValue(undefined);
      signOut.mockResolvedValue(undefined);
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));
      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      await act(async () => {
        await result.current.handleDeleteAccount();
      });

      expect(removeUserAccount).toHaveBeenCalledWith('user-1');
      expect(signOut).toHaveBeenCalledTimes(1);
      expect(clearCurrentUserId).toHaveBeenCalledTimes(1);
      expect(result.current.deletingAccount).toBe(false);
    });

    test('resets deletingAccount back to false even when removeUserAccount throws', async () => {
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      removeUserAccount.mockRejectedValue(new Error('server error'));
      const navigation = createNavigation();
      const { result } = renderHook(() => useProfileViewModel(navigation));
      await waitFor(() => expect(result.current.isLoading()).toBe(false));

      await act(async () => {
        await expect(result.current.handleDeleteAccount()).rejects.toThrow('server error');
      });

      expect(result.current.deletingAccount).toBe(false);
    });
  });
});