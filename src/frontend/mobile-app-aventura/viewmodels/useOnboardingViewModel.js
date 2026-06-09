import { useState } from 'react';
import { onboardingService } from '../services/onboardingService';
import { CATEGORIES } from '../models/onboardingData';

const TOTAL_STEPS = 3;

export function useOnboardingViewModel(onFinish) {
  const [step, setStep] = useState(0);
  const [selectedCategories, setSelectedCategories] = useState([]);
  const [selectedCity, setSelectedCity] = useState(null);
  const [selectedFrequency, setSelectedFrequency] = useState(null);

  function toggleCategory(id) {
    setSelectedCategories((prev) =>
      prev.includes(id) ? prev.filter((c) => c !== id) : [...prev, id]
    );
  }

  function selectCity(id) {
    setSelectedCity(id);
  }

  function selectFrequency(id) {
    setSelectedFrequency(id);
  }

  function canContinue() {
    if (step === 0) return selectedCategories.length > 0;
    if (step === 1) return selectedCity !== null;
    if (step === 2) return selectedFrequency !== null;
    return false;
  }

  async function goNext() {
    if (step < TOTAL_STEPS - 1) {
      setStep((prev) => prev + 1);
      return;
    }
    await _saveAndFinish();
  }

  function skip() {
    if (step < TOTAL_STEPS - 1) {
      setStep((prev) => prev + 1);
      return;
    }
    _saveAndFinish();
  }

  async function _saveAndFinish() {
    try {
      // Send selected category ids (GUIDs) so backend receives genre ids.
      const selectedCategoryIds = CATEGORIES
        .filter((c) => selectedCategories.includes(c.id))
        .map((c) => c.id);

      await onboardingService.save({
        categories: selectedCategoryIds,
        city: selectedCity,
        frequency: selectedFrequency,
      });
    } finally {
      onFinish();
    }
  }

  function goBack() {
    if (step > 0) {
      setStep((prev) => prev - 1);
    }
  }

  return {
    step,
    totalSteps: TOTAL_STEPS,
    selectedCategories,
    selectedCity,
    selectedFrequency,
    toggleCategory,
    selectCity,
    selectFrequency,
    canContinue,
    goNext,
    goBack,
    skip,
  };
}
