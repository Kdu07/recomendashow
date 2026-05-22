import React from 'react';
import { View, Text, TextInput, StyleSheet } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function AppInput({
  label,
  placeholder,
  value,
  onChangeText,
  secureTextEntry = false,
  keyboardType = 'default',
  error,
}) {
  return (
    <View style={styles.container}>
      {label && <Text style={styles.label}>{label}</Text>}

      <TextInput
        style={[styles.input, error && styles.inputError]}
        placeholder={placeholder}
        placeholderTextColor={colors.grayText}
        value={value}
        onChangeText={onChangeText}
        secureTextEntry={secureTextEntry}
        keyboardType={keyboardType}
      />

      {error && <Text style={styles.errorText}>{error}</Text>}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    gap: spacing.xs,
  },

  label: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },

  input: {
    height: 52,
    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,
    paddingHorizontal: spacing.md,
    fontSize: typography.size.md,
    color: colors.grayDark,
    backgroundColor: colors.white,
  },

  inputError: {
    borderColor: colors.peachBurnt,
  },

  errorText: {
    fontSize: typography.size.xs,
    color: colors.peachBurnt,
  },
});