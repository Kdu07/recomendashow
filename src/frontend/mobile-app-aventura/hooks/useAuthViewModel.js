import { useMemo, useState } from 'react';

const initialValues = {
  name: '',
  email: '',
  password: '',
  confirmPassword: '',
};

export function useAuthViewModel() {
  const [mode, setMode] = useState('login');
  const [values, setValues] = useState(initialValues);

  const isLogin = mode === 'login';

  const content = useMemo(() => {
    if (isLogin) {
      return {
        subtitle: 'Entre para descobrir experiencias culturais unicas e personalizadas para voce.',
        submitLabel: 'Entrar',
      };
    }

    return {
      subtitle: 'Crie sua conta para salvar preferencias e receber recomendacoes personalizadas.',
      submitLabel: 'Criar conta',
    };
  }, [isLogin]);

  function updateField(field, value) {
    setValues((currentValues) => ({
      ...currentValues,
      [field]: value,
    }));
  }

  function handleSubmit() {
    // Integração com backend futura aqui
  }

  function handleGoogleAuth() {
    // Integração com Google futura aqui
  }

  return {
    mode,
    values,
    content,
    isLogin,
    setMode,
    updateField,
    handleSubmit,
    handleGoogleAuth,
  };
}