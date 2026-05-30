/**
 * @file useNotificationsViewModel.test.js
 * Testes unitários para useNotificationsViewModel
 */

import { renderHook, act } from '@testing-library/react';
import useNotificationsViewModel from '../viewmodels/useNotificationsViewModel';

// ── Mocks ─────────────────────────────────────────────────────────────────────

const MOCK_SECTIONS = [
  {
    id: 'sec-1',
    title: 'Espetáculos',
    options: [
      { id: 'opt-1', label: 'Novos espetáculos', enabled: true },
      { id: 'opt-2', label: 'Promoções', enabled: false },
    ],
  },
  {
    id: 'sec-2',
    title: 'Lembretes',
    options: [{ id: 'opt-3', label: 'Antes do espetáculo', enabled: true }],
  },
];

jest.mock('../repositories/notificationRepository', () => ({
  getNotificationData: () => ({ sections: MOCK_SECTIONS }),
}));

// ── Testes ────────────────────────────────────────────────────────────────────

describe('useNotificationsViewModel', () => {
  test('carrega as seções do repositório', () => {
    const { result } = renderHook(() => useNotificationsViewModel());
    expect(result.current.sections).toHaveLength(2);
  });

  test('toggleOption inverte o estado enabled da opção', () => {
    const { result } = renderHook(() => useNotificationsViewModel());
    // opt-1 começa true
    act(() => result.current.handleToggleOption('sec-1', 'opt-1'));
    const sec1 = result.current.sections.find((s) => s.id === 'sec-1');
    const opt1 = sec1.options.find((o) => o.id === 'opt-1');
    expect(opt1.enabled).toBe(false);
  });

  test('toggleOption inverte opt desabilitada para habilitada', () => {
    const { result } = renderHook(() => useNotificationsViewModel());
    act(() => result.current.handleToggleOption('sec-1', 'opt-2'));
    const sec1 = result.current.sections.find((s) => s.id === 'sec-1');
    const opt2 = sec1.options.find((o) => o.id === 'opt-2');
    expect(opt2.enabled).toBe(true);
  });

  test('toggleOption não afeta outras seções', () => {
    const { result } = renderHook(() => useNotificationsViewModel());
    act(() => result.current.handleToggleOption('sec-1', 'opt-1'));
    const sec2 = result.current.sections.find((s) => s.id === 'sec-2');
    expect(sec2.options[0].enabled).toBe(true); // inalterado
  });

  test('toggleOption não afeta outras opções da mesma seção', () => {
    const { result } = renderHook(() => useNotificationsViewModel());
    act(() => result.current.handleToggleOption('sec-1', 'opt-1'));
    const sec1 = result.current.sections.find((s) => s.id === 'sec-1');
    const opt2 = sec1.options.find((o) => o.id === 'opt-2');
    expect(opt2.enabled).toBe(false); // inalterado
  });

  test('toggle duplo restaura o estado original', () => {
    const { result } = renderHook(() => useNotificationsViewModel());
    act(() => result.current.handleToggleOption('sec-1', 'opt-1'));
    act(() => result.current.handleToggleOption('sec-1', 'opt-1'));
    const sec1 = result.current.sections.find((s) => s.id === 'sec-1');
    const opt1 = sec1.options.find((o) => o.id === 'opt-1');
    expect(opt1.enabled).toBe(true);
  });
});