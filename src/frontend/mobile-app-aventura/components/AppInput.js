import React, { useState } from 'react';
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
  autoCapitalize = 'sentences',
  autoComplete,
  textContentType,
  returnKeyType,
  autoCorrect = true,
}) {
  const [focused, setFocused] = useState(false);

  return (
    <View style={styles.container}>
      {label && <Text style={styles.label}>{label}</Text>}

      <TextInput
        style={[
          styles.input,
          focused && styles.inputFocused,
          error && styles.inputError,
        ]}
        placeholder={placeholder}
        placeholderTextColor={colors.grayText}
        value={value}
        onChangeText={onChangeText}
        onBlur={() => setFocused(false)}
        onFocus={() => setFocused(true)}
        secureTextEntry={secureTextEntry}
        keyboardType={keyboardType}
        autoCapitalize={autoCapitalize}
        autoComplete={autoComplete}
        textContentType={textContentType}
        returnKeyType={returnKeyType}
        autoCorrect={autoCorrect}
        accessibilityLabel={label}
      />

      {error && <Text style={styles.errorText}>{error}</Text>}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    gap: spacing.sm,
  },

  label: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  input: {
    height: 48,
    borderWidth: 1.5,
    borderColor: colors.grayLight,
    borderRadius: radius.sm,
    paddingHorizontal: spacing.md,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayDark,
    backgroundColor: colors.white,
  },

  inputFocused: {
    borderColor: colors.purplePrimary,
    backgroundColor: colors.white,
  },

  inputError: {
    borderColor: colors.peachBurnt,
  },

  errorText: {
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    color: colors.peachBurnt,
  },
});