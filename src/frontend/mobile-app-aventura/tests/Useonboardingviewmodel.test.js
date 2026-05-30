/**
 * @file useOnboardingViewModel.test.js
 * Testes unitários para useOnboardingViewModel
 *
 * O hook gerencia um fluxo de 3 passos (categorias → cidade → frequência).
 * onboardingService.save é mockado para evitar dependência de AsyncStorage.
 */

import { renderHook, act } from '@testing-library/react';
import { useOnboardingViewModel } from '../viewmodels/useOnboardingViewModel';

// ── Mocks ────────────────────────────────────────────────────────────────────

jest.mock('../services/onboardingService', () => ({
  onboardingService: { save: jest.fn().mockResolvedValue(undefined) },
}));

import { onboardingService } from '../services/onboardingService';

// ── Helpers ──────────────────────────────────────────────────────────────────

function setup(onFinish = jest.fn()) {
  return renderHook(() => useOnboardingViewModel(onFinish));
}

// ── Estado inicial ────────────────────────────────────────────────────────────

describe('useOnboardingViewModel – estado inicial', () => {
  test('inicia no passo 0 com listas vazias', () => {
    const { result } = setup();
    expect(result.current.step).toBe(0);
    expect(result.current.selectedCategories).toEqual([]);
    expect(result.current.selectedCity).toBeNull();
    expect(result.current.selectedFrequency).toBeNull();
    expect(result.current.totalSteps).toBe(3);
  });

  test('canContinue é false sem categorias selecionadas', () => {
    const { result } = setup();
    expect(result.current.canContinue()).toBe(false);
  });
});

// ── toggleCategory ────────────────────────────────────────────────────────────

describe('useOnboardingViewModel – toggleCategory', () => {
  test('adiciona categoria ao selecionar', () => {
    const { result } = setup();
    act(() => result.current.toggleCategory('teatro'));
    expect(result.current.selectedCategories).toContain('teatro');
  });

  test('remove categoria ao selecionar de novo (toggle)', () => {
    const { result } = setup();
    act(() => result.current.toggleCategory('teatro'));
    act(() => result.current.toggleCategory('teatro'));
    expect(result.current.selectedCategories).not.toContain('teatro');
  });

  test('permite múltiplas categorias', () => {
    const { result } = setup();
    act(() => {
      result.current.toggleCategory('teatro');
      result.current.toggleCategory('musical');
    });
    expect(result.current.selectedCategories).toHaveLength(2);
  });

  test('canContinue vira true com pelo menos uma categoria', () => {
    const { result } = setup();
    act(() => result.current.toggleCategory('teatro'));
    expect(result.current.canContinue()).toBe(true);
  });
});

// ── Navegação entre passos ────────────────────────────────────────────────────

describe('useOnboardingViewModel – goNext / goBack', () => {
  test('goNext avança para o passo 1', async () => {
    const { result } = setup();
    act(() => result.current.toggleCategory('teatro'));
    await act(async () => { await result.current.goNext(); });
    expect(result.current.step).toBe(1);
  });

  test('goBack volta para o passo anterior', async () => {
    const { result } = setup();
    act(() => result.current.toggleCategory('teatro'));
    await act(async () => { await result.current.goNext(); });
    act(() => result.current.goBack());
    expect(result.current.step).toBe(0);
  });

  test('goBack no passo 0 não diminui o passo', () => {
    const { result } = setup();
    act(() => result.current.goBack());
    expect(result.current.step).toBe(0);
  });
});

// ── selectCity (passo 1) ──────────────────────────────────────────────────────

describe('useOnboardingViewModel – selectCity', () => {
  test('define a cidade selecionada', () => {
    const { result } = setup();
    act(() => result.current.selectCity('sp'));
    expect(result.current.selectedCity).toBe('sp');
  });

  test('canContinue no passo 1 só é true com cidade selecionada', async () => {
    const { result } = setup();
    act(() => result.current.toggleCategory('teatro'));
    await act(async () => { await result.current.goNext(); });
    expect(result.current.canContinue()).toBe(false);
    act(() => result.current.selectCity('sp'));
    expect(result.current.canContinue()).toBe(true);
  });
});

// ── selectFrequency (passo 2) ─────────────────────────────────────────────────

describe('useOnboardingViewModel – selectFrequency', () => {
  async function goToStep2(result) {
    act(() => result.current.toggleCategory('teatro'));
    await act(async () => { await result.current.goNext(); });
    act(() => result.current.selectCity('sp'));
    await act(async () => { await result.current.goNext(); });
  }

  test('define a frequência selecionada', async () => {
    const { result } = setup();
    await goToStep2(result);
    act(() => result.current.selectFrequency('monthly'));
    expect(result.current.selectedFrequency).toBe('monthly');
  });

  test('canContinue no passo 2 só é true com frequência selecionada', async () => {
    const { result } = setup();
    await goToStep2(result);
    expect(result.current.canContinue()).toBe(false);
    act(() => result.current.selectFrequency('monthly'));
    expect(result.current.canContinue()).toBe(true);
  });
});

// ── Finalização (último passo) ────────────────────────────────────────────────

describe('useOnboardingViewModel – finalização', () => {
  async function fillAndFinish(onFinish) {
    const { result } = setup(onFinish);
    act(() => result.current.toggleCategory('teatro'));
    await act(async () => { await result.current.goNext(); });
    act(() => result.current.selectCity('sp'));
    await act(async () => { await result.current.goNext(); });
    act(() => result.current.selectFrequency('monthly'));
    await act(async () => { await result.current.goNext(); });
    return result;
  }

  test('chama onboardingService.save com os dados corretos', async () => {
    const onFinish = jest.fn();
    await fillAndFinish(onFinish);
    expect(onboardingService.save).toHaveBeenCalledWith({
      categories: ['teatro'],
      city: 'sp',
      frequency: 'monthly',
    });
  });

  test('chama onFinish ao concluir o último passo', async () => {
    const onFinish = jest.fn();
    await fillAndFinish(onFinish);
    expect(onFinish).toHaveBeenCalled();
  });

  test('skip no último passo também chama onFinish', async () => {
    const onFinish = jest.fn();
    const { result } = setup(onFinish);
    // avança até o último passo via skip
    act(() => result.current.skip());
    act(() => result.current.skip());
    await act(async () => { result.current.skip(); });
    expect(onFinish).toHaveBeenCalled();
  });
});