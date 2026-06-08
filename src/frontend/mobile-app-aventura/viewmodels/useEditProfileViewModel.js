import { useState } from 'react';

import { getProfileData, updateProfileData } from '../repositories/profileRepository';

function isValidBirthDate(value) {
  const [day, month, year] = String(value ?? '').split('/').map(Number);

  if (!day || !month || !year) {
    return false;
  }

  const date = new Date(year, month - 1, day);

  return (
    date.getFullYear() === year
    && date.getMonth() === month - 1
    && date.getDate() === day
  );
}

function validateProfile(values) {
  const errors = {};
  const cepDigits = String(values.cep ?? '').replace(/\D/g, '');

  if (!values.name.trim()) {
    errors.name = 'Informe seu nome.';
  }

  if (!values.email.trim()) {
    errors.email = 'Informe seu e-mail.';
  } else if (!values.email.includes('@')) {
    errors.email = 'Informe um e-mail válido.';
  }

  if (!values.birthDate.trim()) {
    errors.birthDate = 'Informe sua data de nascimento.';
  } else if (!isValidBirthDate(values.birthDate)) {
    errors.birthDate = 'Informe uma data válida no formato DD/MM/AAAA.';
  }

  if (!cepDigits) {
    errors.cep = 'Informe seu CEP.';
  } else if (cepDigits.length !== 8) {
    errors.cep = 'Informe um CEP válido com 8 dígitos.';
  }

  if (!values.street.trim()) {
    errors.street = 'Informe sua rua.';
  }

  if (!values.number.trim()) {
    errors.number = 'Informe o número.';
  }

  if (!values.neighborhood.trim()) {
    errors.neighborhood = 'Informe seu bairro.';
  }

  if (!values.city.trim()) {
    errors.city = 'Informe sua cidade.';
  }

  return errors;
}

export default function useEditProfileViewModel() {
  const currentProfile = getProfileData().userProfile;
  const [values, setValues] = useState({
    name: currentProfile.name || '',
    email: currentProfile.email || '',
    birthDate: currentProfile.birthDate || '',
    cep: currentProfile.cep || '',
    street: currentProfile.street || '',
    number: currentProfile.number || '',
    complement: currentProfile.complement || '',
    neighborhood: currentProfile.neighborhood || '',
    city: currentProfile.city || '',
    avatarIcon: currentProfile.avatarIcon || 'person',
  });
  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);

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

  async function saveProfile() {
    const validationErrors = validateProfile(values);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return {
        ok: false,
        reason: 'validation',
      };
    }

    setSaving(true);

    try {
      updateProfileData(values);
      return {
        ok: true,
      };
    } finally {
      setSaving(false);
    }
  }

  return {
    values,
    errors,
    saving,
    updateField,
    saveProfile,
  };
}
