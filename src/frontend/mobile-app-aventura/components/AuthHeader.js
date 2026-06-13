import React from 'react';
import { Image, StyleSheet, Text, View } from 'react-native';

import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';
import { typography } from '../constants/typography';
import aventuraLogo from '../assets/logo-preta-aventura-extenso.png';

export default function AuthHeader({ subtitle }) {
  return (
    <View style={styles.container}>
      <View style={styles.copy}>
        <Image
          source={aventuraLogo}
          style={styles.logo}
          resizeMode="contain"
        />

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

  logo: {
    width: '100%',
    maxWidth: 280,
    height: 58,
  },

  subtitle: {
    maxWidth: 280,
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    color: colors.grayText,
    textAlign: 'center',
  },
});
