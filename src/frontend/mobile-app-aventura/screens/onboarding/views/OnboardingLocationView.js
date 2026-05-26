import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

import SelectableOption from '../components/SelectableOption';
import OnboardingLayout from '../components/OnboardingLayout';
import { CITIES } from '../../../models/onboardingData';
import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing } from '../../../constants/spacing';

export default function OnboardingLocationView({
  step,
  totalSteps,
  selectedCity,
  onSelectCity,
  onContinue,
  onSkip,
  onBack,
  animatedStyle,
}) {
  return (
    <OnboardingLayout
      step={step}
      totalSteps={totalSteps}
      onSkip={onSkip}
      onBack={onBack}
      onContinue={onContinue}
      canContinue={selectedCity !== null}
      animatedStyle={animatedStyle}
    >
      <Text style={styles.title}>Onde você está?</Text>
      <Text style={styles.subtitle}>
        Escolha sua cidade para ver espetáculos perto de você.
      </Text>

      <View style={styles.list}>
        {CITIES.map((city) => (
          <SelectableOption
            key={city.id}
            label={city.label}
            selected={selectedCity === city.id}
            onPress={() => onSelectCity(city.id)}
          />
        ))}
      </View>
    </OnboardingLayout>
  );
}

const styles = StyleSheet.create({
  title: {
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    marginBottom: spacing.sm,
  },

  subtitle: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
    marginBottom: spacing.xl,
    lineHeight: typography.lineHeight.sm,
  },

  list: {
    gap: spacing.sm,
  },
});
