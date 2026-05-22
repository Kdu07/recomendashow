import React from 'react';
import { View, TextInput, StyleSheet, TouchableOpacity, Text } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function SearchBar({
  placeholder = 'Buscar espetáculos, teatros...',
  value,
  onChangeText,
  onFilterPress,
}) {
  return (
    <View style={styles.container}>
      <Text style={styles.icon}>⌕</Text>

      <TextInput
        style={styles.input}
        placeholder={placeholder}
        placeholderTextColor={colors.grayText}
        value={value}
        onChangeText={onChangeText}
      />

      {onFilterPress && (
        <TouchableOpacity onPress={onFilterPress} activeOpacity={0.7}>
          <Text style={styles.filterIcon}>⌯</Text>
        </TouchableOpacity>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    height: 48,
    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,
    backgroundColor: colors.white,
    paddingHorizontal: spacing.md,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },

  icon: {
    fontSize: typography.size.lg,
    color: colors.grayText,
  },

  input: {
    flex: 1,
    fontSize: typography.size.sm,
    color: colors.grayDark,
  },

  filterIcon: {
    fontSize: typography.size.lg,
    color: colors.grayDark,
  },
});