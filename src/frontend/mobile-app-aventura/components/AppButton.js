import React from 'react';
import {
  TouchableOpacity,
  Text,
  StyleSheet,
  ActivityIndicator,
} from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function AppButton({
  title,
  onPress,
  variant = 'primary',
  disabled = false,
  loading = false,
  style,
}) {
  const isPrimary = variant === 'primary';
  const isSecondary = variant === 'secondary';
  const isText = variant === 'text';

  return (
    <TouchableOpacity
      style={[
        styles.button,
        isPrimary && styles.primaryButton,
        isSecondary && styles.secondaryButton,
        isText && styles.textButton,
        disabled && styles.disabledButton,
        style,
      ]}
      onPress={onPress}
      activeOpacity={0.8}
      disabled={disabled || loading}
    >
      {loading ? (
        <ActivityIndicator
          color={isPrimary ? colors.white : colors.purplePrimary}
        />
      ) : (
        <Text
          style={[
            styles.label,
            isPrimary && styles.primaryLabel,
            isSecondary && styles.secondaryLabel,
            isText && styles.textLabel,
          ]}
        >
          {title}
        </Text>
      )}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  button: {
    minHeight: 52,
    borderRadius: radius.md,
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: spacing.lg,
  },

  primaryButton: {
    backgroundColor: colors.purplePrimary,
  },

  secondaryButton: {
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.purplePrimary,
  },

  textButton: {
    minHeight: 40,
    backgroundColor: 'transparent',
    paddingHorizontal: 0,
  },

  disabledButton: {
    opacity: 0.5,
  },

  label: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.semiBold,
  },

  primaryLabel: {
    color: colors.white,
  },

  secondaryLabel: {
    color: colors.purplePrimary,
  },

  textLabel: {
    color: colors.purplePrimary,
  },
});