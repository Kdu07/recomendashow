import { useMemo, useState, useEffect } from 'react';

import { getInterestData } from '../repositories/interestRepository';
import { updatePreferences, getPreferences } from '../repositories/preferencesRepository';

export default function useEditInterestsViewModel() {
  const data = useMemo(
    () => getInterestData(),
    []
  );

  const [interests, setInterests] = useState(
    data.interests || []
  );

  const [cities, setCities] = useState(
    data.cities || []
  );

  // On mount, load server preferences and mark selected interests/city
  useEffect(() => {
    let mounted = true;

    async function loadPrefs() {
      try {
        const prefs = await getPreferences();
        if (!prefs) return;

        const serverGeneros = Array.isArray(prefs.generos) ? prefs.generos : prefs.Generos ?? prefs.generos ?? [];

        if (mounted && serverGeneros.length > 0) {
          setInterests((prev) =>
            prev.map((interest) => ({
              ...interest,
              selected: serverGeneros.includes(interest.id) || serverGeneros.includes(interest.label),
            }))
          );
        }

        const serverCidade = prefs.cidade ?? prefs.Cidade ?? null;
        if (mounted && serverCidade) {
          setCities((prev) => prev.map((city) => ({ ...city, selected: city.label === serverCidade || city.id === serverCidade })));
        }
      } catch (err) {
        console.warn('Erro ao carregar preferências do servidor:', err);
      }
    }

    loadPrefs();
    return () => { mounted = false; };
  }, []);

  function handleToggleInterest(interestId) {
    setInterests((previous) =>
      previous.map((interest) => {
        if (interest.id === interestId) {
          return {
            ...interest,
            selected: !interest.selected,
          };
        }

        return interest;
      })
    );
  }

  function handleSelectCity(cityId) {
    setCities((previous) =>
      previous.map((city) => ({
        ...city,
        selected: city.id === cityId,
      }))
    );
  }

  const selectedInterestsCount =
    interests.filter(
      (interest) => interest.selected
    ).length;

  async function handleSaveChanges() {
    console.log('Salvar alterações');

    const selectedGenres = interests.filter(i => i.selected).map(i => i.id);

    const payload = {
      Generos: selectedGenres,
      Cidade: cities.find((city) => city.selected)?.label,
    };

    const result = await updatePreferences(payload);

    return {
      selectedInterestsCount,
      selectedCity: payload.Cidade,
      savedOnServer: !!result,
    };
  }

  return {
    interests,
    cities,

    selectedInterestsCount,

    handleToggleInterest,
    handleSelectCity,
    handleSaveChanges,
  };
}
