import { renderHook, act, waitFor } from '@testing-library/react';

import useNotificationsViewModel from '../viewmodels/useNotificationsViewModel';

const MOCK_SECTIONS = [
  {
    id: 'sec-1',
    title: 'Espetaculos',
    options: [
      { id: 'opt-1', title: 'Novos espetaculos', enabled: true },
      { id: 'opt-2', title: 'Promocoes', enabled: false },
    ],
  },
  {
    id: 'sec-2',
    title: 'Lembretes',
    options: [{ id: 'opt-3', title: 'Antes do espetaculo', enabled: true }],
  },
];

const mockUpdateNotificationPreference = jest.fn(() => true);

jest.mock('../repositories/notificationRepository', () => ({
  getNotificationData: () => ({
    sections: MOCK_SECTIONS.map((section) => ({
      ...section,
      options: section.options.map((option) => ({ ...option })),
    })),
  }),
  updateNotificationPreference: (...args) => mockUpdateNotificationPreference(...args),
}));

async function renderLoadedHook() {
  const { result } = renderHook(() => useNotificationsViewModel());
  await waitFor(() => expect(result.current.sections).toHaveLength(2));
  return result;
}

describe('useNotificationsViewModel', () => {
  beforeEach(() => {
    mockUpdateNotificationPreference.mockClear();
    mockUpdateNotificationPreference.mockReturnValue(true);
  });

  test('carrega as secoes do repositorio', async () => {
    const result = await renderLoadedHook();
    expect(result.current.sections).toHaveLength(2);
  });

  test('toggleOption inverte o estado enabled da opcao e salva no repositorio', async () => {
    const result = await renderLoadedHook();

    await act(async () => {
      await result.current.handleToggleOption('sec-1', 'opt-1');
    });

    const sec1 = result.current.sections.find((section) => section.id === 'sec-1');
    const opt1 = sec1.options.find((option) => option.id === 'opt-1');

    expect(opt1.enabled).toBe(false);
    expect(mockUpdateNotificationPreference).toHaveBeenCalledWith(false);
  });

  test('toggleOption inverte opcao desabilitada para habilitada', async () => {
    const result = await renderLoadedHook();

    await act(async () => {
      await result.current.handleToggleOption('sec-1', 'opt-2');
    });

    const sec1 = result.current.sections.find((section) => section.id === 'sec-1');
    const opt2 = sec1.options.find((option) => option.id === 'opt-2');

    expect(opt2.enabled).toBe(true);
    expect(mockUpdateNotificationPreference).toHaveBeenCalledWith(true);
  });

  test('toggleOption nao afeta outras secoes', async () => {
    const result = await renderLoadedHook();

    await act(async () => {
      await result.current.handleToggleOption('sec-1', 'opt-1');
    });

    const sec2 = result.current.sections.find((section) => section.id === 'sec-2');
    expect(sec2.options[0].enabled).toBe(true);
  });

  test('toggleOption nao afeta outras opcoes da mesma secao', async () => {
    const result = await renderLoadedHook();

    await act(async () => {
      await result.current.handleToggleOption('sec-1', 'opt-1');
    });

    const sec1 = result.current.sections.find((section) => section.id === 'sec-1');
    const opt2 = sec1.options.find((option) => option.id === 'opt-2');
    expect(opt2.enabled).toBe(false);
  });

  test('toggle duplo restaura o estado original', async () => {
    const result = await renderLoadedHook();

    await act(async () => {
      await result.current.handleToggleOption('sec-1', 'opt-1');
    });
    await act(async () => {
      await result.current.handleToggleOption('sec-1', 'opt-1');
    });

    const sec1 = result.current.sections.find((section) => section.id === 'sec-1');
    const opt1 = sec1.options.find((option) => option.id === 'opt-1');
    expect(opt1.enabled).toBe(true);
  });

  test('restaura estado anterior quando nao consegue salvar', async () => {
    mockUpdateNotificationPreference.mockReturnValue(false);
    const result = await renderLoadedHook();

    await act(async () => {
      await result.current.handleToggleOption('sec-1', 'opt-1');
    });

    const sec1 = result.current.sections.find((section) => section.id === 'sec-1');
    const opt1 = sec1.options.find((option) => option.id === 'opt-1');
    expect(opt1.enabled).toBe(true);
  });
});
