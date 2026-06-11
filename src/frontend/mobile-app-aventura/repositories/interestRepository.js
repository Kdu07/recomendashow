import { fetchCategories } from '../services/categoryService';
import { getPreferences, updatePreferences } from './preferencesRepository';

const DEFAULT_CITIES = [
  { id: 'sao_paulo', label: 'Sao Paulo' },
  { id: 'rio_de_janeiro', label: 'Rio de Janeiro' },
  { id: 'outra', label: 'Outra' },
];

function normalize(value) {
  return String(value ?? '').trim().toLowerCase();
}

export async function getInterestData() {
  const [categories, preferences] = await Promise.all([
    fetchCategories(),
    getPreferences(),
  ]);

  const savedGeneros = preferences?.Generos ?? preferences?.generos ?? [];
  const savedCidade = preferences?.Cidade ?? preferences?.cidade ?? null;

  const interests = categories
    .filter((category) => category.id !== 'all')
    .map((category) => ({
      id: category.label,
      label: category.label,
      selected: savedGeneros.some((genero) => normalize(genero) === normalize(category.label)),
    }));

  const cities = DEFAULT_CITIES.map((city) => ({
    ...city,
    selected: normalize(city.label) === normalize(savedCidade),
  }));

  return {
    interests,
    cities,
    preferences,
  };
}

export async function saveInterestData({ interests, cities }) {
  const preferences = await getPreferences();
  const selectedGeneros = interests
    .filter((interest) => interest.selected)
    .map((interest) => interest.label);
  const selectedCity = cities.find((city) => city.selected)?.label ?? null;

  const updated = await updatePreferences({
    Generos: selectedGeneros,
    Tags: preferences?.Tags ?? preferences?.tags ?? [],
    Cep: preferences?.Cep ?? preferences?.cep ?? null,
    Cidade: selectedCity,
    FaixaPrecoMin: preferences?.FaixaPrecoMin ?? preferences?.faixaPrecoMin ?? null,
    FaixaPrecoMax: preferences?.FaixaPrecoMax ?? preferences?.faixaPrecoMax ?? null,
    ReceberNotificacoes: preferences?.ReceberNotificacoes ?? preferences?.receberNotificacoes ?? true,
  });

  return {
    updated,
    selectedInterestsCount: selectedGeneros.length,
    selectedCity,
    savedOnServer: Boolean(updated),
  };
}
