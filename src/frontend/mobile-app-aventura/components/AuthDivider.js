import React from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';
import { typography } from '../constants/typography';

export default function AuthDivider({ label = 'ou continue com' }) {
  return (
    <View style={styles.container}>
      <View style={styles.line} />
      <Text style={styles.label}>{label}</Text>
      <View style={styles.line} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
    paddingVertical: spacing.xs,
  },

  line: {
    flex: 1,
    height: 1,
    backgroundColor: colors.grayLight,
  },

  label: {
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    color: colors.grayText,
  },
});
