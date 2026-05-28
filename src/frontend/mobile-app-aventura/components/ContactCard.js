import React from 'react';

import {
  View,
  Text,
  StyleSheet,
} from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function ContactCard({
  icon,
  value,
  link = false,
}) {
  return (
    <View style={styles.container}>
      <Text style={styles.icon}>
        {icon}
      </Text>

      <Text
        style={[
          styles.value,
          link && styles.link,
        ]}
      >
        {value}
      </Text>

      {link && (
        <Text style={styles.external}>
          ↗
        </Text>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    minHeight: 72,

    borderWidth: 1,
    borderColor: colors.grayLight,

    borderRadius: radius.lg,

    backgroundColor: colors.white,

    paddingHorizontal: spacing.md,

    flexDirection: 'row',
    alignItems: 'center',

    gap: spacing.md,
  },

  icon: {
    fontSize: 22,
    color: colors.purplePrimary,
  },

  value: {
    flex: 1,

    fontSize: typography.size.md,
    color: colors.grayDark,

    lineHeight: 26,
  },

  link: {
    color: colors.purplePrimary,
    textDecorationLine: 'underline',
  },

  external: {
    fontSize: 22,
    color: colors.grayText,
  },
});