import {
  setCurrentUserId,
  getCurrentUserId,
  clearCurrentUserId,
} from '../services/userSession';

describe('userSession', () => {
  afterEach(() => {
    clearCurrentUserId();
  });

  test('returns null when no user id has been set', () => {
    expect(getCurrentUserId()).toBeNull();
  });

  test('stores and returns the current user id', () => {
    setCurrentUserId('user-123');

    expect(getCurrentUserId()).toBe('user-123');
  });

  test('overwrites the previously stored user id', () => {
    setCurrentUserId('user-123');
    setCurrentUserId('user-456');

    expect(getCurrentUserId()).toBe('user-456');
  });

  test('clears the stored user id back to null', () => {
    setCurrentUserId('user-123');
    clearCurrentUserId();

    expect(getCurrentUserId()).toBeNull();
  });
});