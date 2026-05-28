import React from 'react';

import {
  View,
  Text,
  StyleSheet,
  Switch,
} from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';

export default function NotificationToggleItem({
  title,
  subtitle,
  enabled,
  onToggle,
  showDivider = true,
}) {
  return (
    <View>
      <View style={styles.container}>
        <View style={styles.content}>
          <Text style={styles.title}>
            {title}
          </Text>

          <Text style={styles.subtitle}>
            {subtitle}
          </Text>
        </View>

        <Switch
          value={enabled}
          onValueChange={onToggle}
          trackColor={{
            false: '#E0E0E0',
            true: colors.purplePrimary,
          }}
          thumbColor={colors.white}
        />
      </View>

      {showDivider && (
        <View style={styles.divider} />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',

    gap: spacing.md,
  },

  content: {
    flex: 1,
    gap: 2,
  },

  title: {
    fontSize: typography.size.lg,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },

  subtitle: {
    fontSize: typography.size.md,
    color: colors.grayText,
  },

  divider: {
    height: 1,
    backgroundColor: colors.grayLight,
    marginVertical: spacing.lg,
  },
});