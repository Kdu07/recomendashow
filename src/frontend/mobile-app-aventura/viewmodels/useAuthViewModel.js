import { useMemo, useState } from 'react';

import {
  authenticateUser,
  authenticateWithGoogle,
  createUserAccount,
} from '../repositories/authRepository';
import { AUTH_MODES, initialAuthValues } from '../models/authModels';

function parseBrazilianDate(value) {
  const [day, month, year] = String(value ?? '').split('/').map(Number);

  if (!day || !month || !year) {
    return null;
  }

  const date = new Date(year, month - 1, day);
  const isValidDate = (
    date.getFullYear() === year
    && date.getMonth() === month - 1
    && date.getDate() === day
  );

  return isValidDate ? date : null;
}

function getAgeFromDate(date) {
  const today = new Date();
  let age = today.getFullYear() - date.getFullYear();
  const hasHadBirthdayThisYear = (
    today.getMonth() > date.getMonth()
    || (today.getMonth() === date.getMonth() && today.getDate() >= date.getDate())
  );

  if (!hasHadBirthdayThisYear) {
    age -= 1;
  }

  return age;
}

function validateAuthValues(mode, values, scope = 'all') {
  const errors = {};
  const isLogin = mode === AUTH_MODES.login;
  const cepDigits = String(values.cep ?? '').replace(/\D/g, '');
  const shouldValidateAccount = scope === 'all' || scope === 'account';
  const shouldValidateAddress = scope === 'all' || scope === 'address';

  if (!isLogin && shouldValidateAccount && !values.name.trim()) {
    errors.name = 'Informe seu nome completo.';
  }

  if (!isLogin && shouldValidateAccount) {
    const birthDateValue = String(values.birthDate ?? '').trim();
    const birthDate = parseBrazilianDate(birthDateValue);

    if (!birthDateValue) {
      errors.birthDate = 'Informe sua data de nascimento.';
    } else if (!birthDate) {
      errors.birthDate = 'Informe uma data válida no formato DD/MM/AAAA.';
    } else if (getAgeFromDate(birthDate) < 13) {
      errors.birthDate = 'Você precisa ter pelo menos 13 anos.';
    }
  }

  if (!isLogin && shouldValidateAddress) {
    if (!cepDigits) {
      errors.cep = 'Informe seu CEP.';
    } else if (cepDigits.length !== 8) {
      errors.cep = 'Informe um CEP válido com 8 dígitos.';
    }

    if (!String(values.street ?? '').trim()) {
      errors.street = 'Informe sua rua.';
    }

    if (!String(values.number ?? '').trim()) {
      errors.number = 'Informe o número.';
    }

    if (!String(values.neighborhood ?? '').trim()) {
      errors.neighborhood = 'Informe seu bairro.';
    }

    if (!String(values.city ?? '').trim()) {
      errors.city = 'Informe sua cidade.';
    }
  }

  if (shouldValidateAccount && !values.email.trim()) {
    errors.email = 'Informe seu e-mail.';
  } else if (shouldValidateAccount && !isLogin && !values.email.includes('@')) {
    errors.email = 'Informe um e-mail válido.';
  }

  if (shouldValidateAccount && !values.password) {
    errors.password = 'Informe sua senha.';
  } else if (shouldValidateAccount && !isLogin && values.password.length < 6) {
    errors.password = 'A senha deve ter no mínimo 6 caracteres.';
  }

  if (shouldValidateAccount && !isLogin && values.password !== values.confirmPassword) {
    errors.confirmPassword = 'As senhas precisam ser iguais.';
  }

  return errors;
}

export function useAuthViewModel(navigation) {
  const [mode, setMode] = useState(AUTH_MODES.login);
  const [values, setValues] = useState(initialAuthValues);
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);

  const isLogin = mode === AUTH_MODES.login;

  const content = useMemo(() => {
    if (isLogin) {
      return {
        subtitle: 'Entre para descobrir experiências culturais únicas e personalizadas para você.',
        submitLabel: 'Entrar',
      };
    }

    return {
      subtitle: 'Crie sua conta para salvar preferências e receber recomendações personalizadas.',
      submitLabel: 'Criar conta',
    };
  }, [isLogin]);

  function handleModeChange(nextMode) {
    setMode(nextMode);
    setErrors({});
  }

  function updateField(field, value) {
    setValues((currentValues) => ({
      ...currentValues,
      [field]: value,
    }));

    setErrors((currentErrors) => ({
      ...currentErrors,
      [field]: undefined,
    }));
  }

  async function handleSubmit() {
    const validationErrors = validateAuthValues(mode, values);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return {
        ok: false,
        reason: 'validation',
      };
    }

    setLoading(true);

    try {
      if (isLogin) {
        await authenticateUser({
          email: values.email,
          password: values.password,
        });
      } else {
        await createUserAccount({
          name: values.name,
          email: values.email,
          birthDate: values.birthDate,
          cep: values.cep.replace(/\D/g, ''),
          street: values.street,
          number: values.number,
          complement: values.complement,
          neighborhood: values.neighborhood,
          city: values.city,
          password: values.password,
        });
      }
      navigation.replace('Onboarding');
      return {
        ok: true,
      };
    } finally {
      setLoading(false);
    }
  }

  function validateRegisterStep(scope) {
    const validationErrors = validateAuthValues(mode, values, scope);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return {
        ok: false,
        reason: 'validation',
      };
    }

    setErrors({});
    return {
      ok: true,
    };
  }

  async function handleGoogleAuth() {
    setLoading(true);

    try {
      await authenticateWithGoogle();
    } finally {
      setLoading(false);
    }
  }

  return {
    mode,
    values,
    errors,
    content,
    isLogin,
    loading,
    setMode: handleModeChange,
    updateField,
    validateRegisterStep,
    handleSubmit,
    handleGoogleAuth,
  };
}
