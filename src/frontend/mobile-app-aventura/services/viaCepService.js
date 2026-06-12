import { cachedFetch } from './vhsService';

const VIACEP_BASE_URL = 'https://viacep.com.br/ws';

export function onlyCepDigits(value) {
  return String(value ?? '').replace(/\D/g, '').slice(0, 8);
}

export function formatCep(value) {
  const digits = onlyCepDigits(value);

  if (digits.length <= 5) {
    return digits;
  }

  return `${digits.slice(0, 5)}-${digits.slice(5)}`;
}

export async function lookupAddressByCep(value) {
  const cep = onlyCepDigits(value);

  if (cep.length !== 8) {
    throw new Error('Informe um CEP válido com 8 dígitos.');
  }

  const data = await cachedFetch(
    `${VIACEP_BASE_URL}/${cep}/json/`
  );

  if (data?.erro) {
    throw new Error('CEP não encontrado.');
  }

  return {
    cep: data.cep ?? formatCep(cep),
    street: data.logradouro ?? '',
    neighborhood: data.bairro ?? '',
    city: data.localidade ?? '',
    state: data.uf ?? '',
  };
}
