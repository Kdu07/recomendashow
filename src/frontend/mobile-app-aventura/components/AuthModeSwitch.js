import React from 'react';
import { StyleSheet, Text, TouchableOpacity } from 'react-native';

import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';
import { typography } from '../constants/typography';

export default function AuthModeSwitch({ mode, onChange }) {
  const isLogin = mode === 'login';
  const label = isLogin ? 'Criar conta' : 'Ja tenho uma conta';
  const nextMode = isLogin ? 'register' : 'login';

  return (
    <TouchableOpacity
      style={styles.container}
      onPress={() => onChange(nextMode)}
      activeOpacity={0.75}
      accessibilityRole="button"
      accessibilityLabel={label}
    >
      <Text style={styles.label}>{label}</Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
    marginTop: 'auto',
    paddingVertical: spacing.md,
  },

  label: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    fontWeight: typography.fontWeight.medium,
    color: colors.purplePrimary,
  },
});
