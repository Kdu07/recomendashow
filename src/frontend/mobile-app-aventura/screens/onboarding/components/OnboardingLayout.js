import React from 'react';
import { View, StyleSheet, SafeAreaView, Animated } from 'react-native';

import AppButton from '../../../components/AppButton';
import { colors } from '../../../constants/colors';
import { spacing } from '../../../constants/spacing';

export default function OnboardingLayout({
  step,
  totalSteps,
  onSkip,
  onBack,
  onContinue,
  canContinue,
  animatedStyle,
  children,
}) {
  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.header}>
        <View style={styles.dots}>
          {Array.from({ length: totalSteps }).map((_, i) => (
            <View key={i} style={[styles.dot, i === step && styles.dotActive]} />
          ))}
        </View>
        <AppButton title="Pular" variant="text" onPress={onSkip} />
      </View>

      <Animated.View style={[styles.content, animatedStyle]}>
        {children}
      </Animated.View>

      <View style={styles.footer}>
        {step > 0 && (
          <AppButton
            title="voltar"
            variant="secondary"
            onPress={onBack}
            style={styles.backButton}
          />
        )}
        <AppButton
          title="Continuar"
          variant="primary"
          onPress={onContinue}
          disabled={!canContinue}
          style={styles.continueButton}
        />
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.white,
  },

  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.lg,
  },

  dots: {
    flexDirection: 'row',
    gap: spacing.xs,
  },

  dot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: colors.grayLight,
  },

  dotActive: {
    backgroundColor: colors.purplePrimary,
  },

  content: {
    flex: 1,
    paddingHorizontal: spacing.xl,
    justifyContent: 'center',
  },

  footer: {
    flexDirection: 'row',
    gap: spacing.sm,
    paddingHorizontal: spacing.xl,
    paddingBottom: spacing.xl,
  },

  backButton: {
    flex: 1,
  },

  continueButton: {
    flex: 2,
  },
});
