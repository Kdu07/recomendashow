import { useEffect, useState } from 'react';

import { getInterestData, saveInterestData } from '../repositories/interestRepository';

export default function useEditInterestsViewModel() {
  const [interests, setInterests] = useState([]);
  const [cities, setCities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    let mounted = true;

    async function load() {
      try {
        const data = await getInterestData();
        if (!mounted) return;
        setInterests(data.interests || []);
        setCities(data.cities || []);
      } catch (error) {
        console.warn('Erro ao carregar interesses:', error);
      } finally {
        if (mounted) setLoading(false);
      }
    }

    load();

    return () => {
      mounted = false;
    };
  }, []);

  function handleToggleInterest(interestId) {
    setInterests((previous) =>
      previous.map((interest) =>
        interest.id === interestId
          ? { ...interest, selected: !interest.selected }
          : interest
      )
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

  const selectedInterestsCount = interests.filter((interest) => interest.selected).length;

  async function handleSaveChanges() {
    setSaving(true);
    try {
      return await saveInterestData({ interests, cities });
    } finally {
      setSaving(false);
    }
  }

  return {
    interests,
    cities,
    loading,
    saving,
    selectedInterestsCount,
    handleToggleInterest,
    handleSelectCity,
    handleSaveChanges,
  };
}
