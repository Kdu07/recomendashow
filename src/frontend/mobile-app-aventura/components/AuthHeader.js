import React from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';
import { typography } from '../constants/typography';

export default function AuthHeader({ subtitle, mode }) {
  const isLogin = mode === 'login';

  return (
    <View style={styles.container}>
      <View style={styles.copy}>
        <Text style={[styles.brand, !isLogin && styles.brandRegister]}>
          AVENTURA
        </Text>
        <Text style={styles.subtitle}>{subtitle}</Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
    paddingTop: spacing.xl,
  },

  copy: {
    width: '100%',
    alignItems: 'center',
    gap: spacing.sm,
  },

  brand: {
    fontSize: typography.size.xl,
    lineHeight: typography.lineHeight.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.black,
  },

  brandRegister: {
    fontSize: typography.size.xxl,
    lineHeight: typography.lineHeight.xxl,
    fontWeight: '900',
  },

  subtitle: {
    maxWidth: 280,
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    color: colors.grayText,
    textAlign: 'center',
  },
});