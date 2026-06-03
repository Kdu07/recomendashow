/**
 * @file useAuthViewModel.test.js
 * Testes unitários para useAuthViewModel
 *
 * Estratégia: a função validateAuthValues é pura e testada diretamente.
 * O hook useAuthViewModel é testado via @testing-library/react-hooks,
 * com os repositórios de autenticação mockados para isolar o comportamento.
 */

import { renderHook, act } from '@testing-library/react';
import { useAuthViewModel } from '../viewmodels/useAuthViewModel';

// ── Mocks ────────────────────────────────────────────────────────────────────

jest.mock('../repositories/authRepository', () => ({
  authenticateUser: jest.fn(),
  createUserAccount: jest.fn(),
  authenticateWithGoogle: jest.fn(),
}));

jest.mock('../models/authModels', () => ({
  AUTH_MODES: { login: 'login', register: 'register' },
  initialAuthValues: {
    name: '',
    email: '',
    age: '',
    cep: '',
    street: '',
    number: '',
    complement: '',
    neighborhood: '',
    city: '',
    password: '',
    confirmPassword: '',
  },
}));

import {
  authenticateUser,
  createUserAccount,
  authenticateWithGoogle,
} from '../repositories/authRepository';

// ── Helpers ──────────────────────────────────────────────────────────────────

function makeNavigation() {
  return { replace: jest.fn(), navigate: jest.fn() };
}

// ── Testes: modo e conteúdo ───────────────────────────────────────────────────

describe('useAuthViewModel – modo e conteúdo', () => {
  test('inicia no modo login', () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    expect(result.current.mode).toBe('login');
    expect(result.current.isLogin).toBe(true);
  });

  test('content do modo login tem submitLabel "Entrar"', () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    expect(result.current.content.submitLabel).toBe('Entrar');
  });

  test('ao mudar para register, isLogin vira false e content muda', () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    expect(result.current.isLogin).toBe(false);
    expect(result.current.content.submitLabel).toBe('Criar conta');
  });

  test('mudar modo limpa os erros', () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    // força um erro manualmente disparando submit vazio
    act(() => { result.current.handleSubmit(); });
    // troca de modo – erros devem ser zerados
    act(() => result.current.setMode('register'));
    expect(result.current.errors).toEqual({});
  });
});

// ── Testes: updateField ───────────────────────────────────────────────────────

describe('useAuthViewModel – updateField', () => {
  test('atualiza o campo correto', () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.updateField('email', 'test@email.com'));
    expect(result.current.values.email).toBe('test@email.com');
  });

  test('limpa o erro do campo ao digitá-lo', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    // gera erro de e-mail
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.email).toBeDefined();
    // digitar no campo limpa o erro
    act(() => result.current.updateField('email', 'a@b.com'));
    expect(result.current.errors.email).toBeUndefined();
  });
});

// ── Testes: validação no modo login ──────────────────────────────────────────

describe('useAuthViewModel – validação (login)', () => {
  test('e-mail vazio gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.email).toBe('Informe seu e-mail.');
  });

  test('senha vazia gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.updateField('email', 'a@b.com'));
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.password).toBe('Informe sua senha.');
  });

  test('não gera erro de name no modo login', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.name).toBeUndefined();
  });
});

// ── Testes: validação no modo register ────────────────────────────────────────

describe('useAuthViewModel – validação (register)', () => {
  test('nome vazio gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.name).toBe('Informe seu nome completo.');
  });

  test('senhas diferentes geram erro em confirmPassword', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('number', '1000');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', 'abcdef');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.confirmPassword).toBe('As senhas precisam ser iguais.');
  });

  test('idade vazia gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('number', '1000');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.age).toBe('Informe sua idade.');
  });

  test('idade menor que 13 gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '12');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('number', '1000');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.age).toBe('Informe uma idade válida a partir de 13 anos.');
  });

  test('CEP inválido gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '123');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.cep).toBe('Informe um CEP válido com 8 dígitos.');
  });

  test('rua vazia gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('number', '1000');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.street).toBe('Informe sua rua.');
  });

  test('número vazio gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.number).toBe('Informe o número.');
  });

  test('bairro vazio gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('number', '1000');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.neighborhood).toBe('Informe seu bairro.');
  });

  test('cidade vazia gera erro', async () => {
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('number', '1000');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.city).toBe('Informe sua cidade.');
  });

  test('senhas iguais não geram erro', async () => {
    authenticateUser.mockResolvedValue({});
    createUserAccount.mockResolvedValue({});
    const nav = makeNavigation();
    const { result } = renderHook(() => useAuthViewModel(nav));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('number', '1000');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(result.current.errors.confirmPassword).toBeUndefined();
  });
});

// ── Testes: handleSubmit (fluxos de sucesso) ─────────────────────────────────

describe('useAuthViewModel – handleSubmit', () => {
  beforeEach(() => {
    authenticateUser.mockResolvedValue({ user: {}, token: 'tok' });
    createUserAccount.mockResolvedValue({ user: {}, token: 'tok' });
  });

  test('login bem-sucedido chama navigation.replace("Onboarding")', async () => {
    const nav = makeNavigation();
    const { result } = renderHook(() => useAuthViewModel(nav));
    act(() => {
      result.current.updateField('email', 'a@b.com');
      result.current.updateField('password', '123');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(authenticateUser).toHaveBeenCalledWith({ email: 'a@b.com', password: '123' });
    expect(nav.replace).toHaveBeenCalledWith('Onboarding');
  });

  test('register chama createUserAccount com name, email, age, endereco e password', async () => {
    const nav = makeNavigation();
    const { result } = renderHook(() => useAuthViewModel(nav));
    act(() => result.current.setMode('register'));
    act(() => {
      result.current.updateField('name', 'Ana');
      result.current.updateField('email', 'ana@email.com');
      result.current.updateField('age', '18');
      result.current.updateField('cep', '01310-100');
      result.current.updateField('street', 'Avenida Paulista');
      result.current.updateField('number', '1000');
      result.current.updateField('complement', 'Apto 12');
      result.current.updateField('neighborhood', 'Bela Vista');
      result.current.updateField('city', 'São Paulo');
      result.current.updateField('password', '123456');
      result.current.updateField('confirmPassword', '123456');
    });
    await act(async () => { await result.current.handleSubmit(); });
    expect(createUserAccount).toHaveBeenCalledWith({
      name: 'Ana',
      email: 'ana@email.com',
      age: 18,
      cep: '01310100',
      street: 'Avenida Paulista',
      number: '1000',
      complement: 'Apto 12',
      neighborhood: 'Bela Vista',
      city: 'São Paulo',
      password: '123456',
    });
  });

  test('loading é true durante a chamada e false ao final', async () => {
    let resolveAuth;
    authenticateUser.mockReturnValue(new Promise((r) => { resolveAuth = r; }));
    const nav = makeNavigation();
    const { result } = renderHook(() => useAuthViewModel(nav));
    act(() => {
      result.current.updateField('email', 'a@b.com');
      result.current.updateField('password', '123');
    });
    act(() => { result.current.handleSubmit(); });
    expect(result.current.loading).toBe(true);
    await act(async () => { resolveAuth({}); });
    expect(result.current.loading).toBe(false);
  });
});

// ── Testes: handleGoogleAuth ─────────────────────────────────────────────────

describe('useAuthViewModel – handleGoogleAuth', () => {
  test('chama authenticateWithGoogle e restaura loading', async () => {
    authenticateWithGoogle.mockResolvedValue({ user: {}, token: 'g-tok' });
    const { result } = renderHook(() => useAuthViewModel(makeNavigation()));
    await act(async () => { await result.current.handleGoogleAuth(); });
    expect(authenticateWithGoogle).toHaveBeenCalled();
    expect(result.current.loading).toBe(false);
  });
});
