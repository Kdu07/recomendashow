/**
 * @file useEditInterestsViewModel.test.js
 * Testes unitários para useEditInterestsViewModel
 */

import { renderHook, act, waitFor } from '@testing-library/react';

// ── Mocks ─────────────────────────────────────────────────────────────────────

const MOCK_INTERESTS = [
  { id: 'teatro', label: 'Teatro', selected: false },
  { id: 'musical', label: 'Musical', selected: true },
  { id: 'danca', label: 'Dança', selected: false },
];

const MOCK_CITIES = [
  { id: 'sp', label: 'São Paulo', selected: true },
  { id: 'rj', label: 'Rio de Janeiro', selected: false },
];

jest.mock('../repositories/interestRepository', () => ({
  getInterestData: jest.fn().mockResolvedValue({
    interests: [
      { id: 'teatro', label: 'Teatro', selected: false },
      { id: 'musical', label: 'Musical', selected: true },
      { id: 'danca', label: 'Dança', selected: false },
    ],
    cities: [
      { id: 'sp', label: 'São Paulo', selected: true },
      { id: 'rj', label: 'Rio de Janeiro', selected: false },
    ],
  }),
  saveInterestData: jest.fn().mockResolvedValue({ savedOnServer: true }),
}));

const useEditInterestsViewModel = require('../viewmodels/useEditInterestsViewModel').default;

// ── Testes ────────────────────────────────────────────────────────────────────

describe('useEditInterestsViewModel', () => {
  async function renderLoadedHook() {
    const { result } = renderHook(() => useEditInterestsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));
    return result;
  }

  test('carrega interesses e cidades do repositório', async () => {
    const result = await renderLoadedHook();
    expect(result.current.interests).toHaveLength(3);
    expect(result.current.cities).toHaveLength(2);
  });

  test('selectedInterestsCount conta apenas os selecionados', async () => {
    const result = await renderLoadedHook();
    // musical começa selecionado
    expect(result.current.selectedInterestsCount).toBe(1);
  });

  test('handleToggleInterest seleciona um interesse desmarcado', async () => {
    const result = await renderLoadedHook();
    act(() => result.current.handleToggleInterest('teatro'));
    const teatro = result.current.interests.find((i) => i.id === 'teatro');
    expect(teatro.selected).toBe(true);
  });

  test('handleToggleInterest desmarca um interesse selecionado', async () => {
    const result = await renderLoadedHook();
    act(() => result.current.handleToggleInterest('musical'));
    const musical = result.current.interests.find((i) => i.id === 'musical');
    expect(musical.selected).toBe(false);
  });

  test('toggle não afeta outros interesses', async () => {
    const result = await renderLoadedHook();
    act(() => result.current.handleToggleInterest('teatro'));
    const danca = result.current.interests.find((i) => i.id === 'danca');
    expect(danca.selected).toBe(false);
  });

  test('selectedInterestsCount é atualizado após toggle', async () => {
    const result = await renderLoadedHook();
    act(() => result.current.handleToggleInterest('teatro'));
    expect(result.current.selectedInterestsCount).toBe(2);
  });

  test('handleSelectCity seleciona apenas a cidade informada', async () => {
    const result = await renderLoadedHook();
    act(() => result.current.handleSelectCity('rj'));
    const rj = result.current.cities.find((c) => c.id === 'rj');
    const sp = result.current.cities.find((c) => c.id === 'sp');
    expect(rj.selected).toBe(true);
    expect(sp.selected).toBe(false);
  });

  test('handleSelectCity mantém apenas uma cidade selecionada', async () => {
    const result = await renderLoadedHook();
    act(() => result.current.handleSelectCity('sp'));
    act(() => result.current.handleSelectCity('rj'));
    const selectedCities = result.current.cities.filter((c) => c.selected);
    expect(selectedCities).toHaveLength(1);
    expect(selectedCities[0].id).toBe('rj');
  });
});
