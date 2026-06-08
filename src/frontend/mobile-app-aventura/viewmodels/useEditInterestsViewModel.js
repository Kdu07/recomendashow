import { useMemo, useState } from 'react';

import { getInterestData } from '../repositories/interestRepository';

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

  function handleSaveChanges() {
    console.log('Salvar alterações');

    return {
      selectedInterestsCount,
      selectedCity: cities.find((city) => city.selected)?.label,
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
