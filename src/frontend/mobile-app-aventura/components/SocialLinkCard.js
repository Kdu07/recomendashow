import React from 'react';

import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
} from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';

export default function SocialLinkCard({
  icon,
  platform,
  username,
  onPress,
  showDivider = true,
}) {
  return (
    <TouchableOpacity
      style={styles.container}
      onPress={onPress}
      activeOpacity={0.8}
    >
      <View style={styles.left}>
        <Text style={styles.icon}>
          {icon}
        </Text>

        <Text style={styles.platform}>
          {platform}
          <Text style={styles.username}>
            {username}
          </Text>
        </Text>
      </View>

      <Text style={styles.external}>
        ↗
      </Text>

      {showDivider && (
        <View style={styles.divider} />
      )}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.lg,
  },

  left: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },

  icon: {
    fontSize: 22,
    color: colors.purplePrimary,
    width: 24,
  },

  platform: {
    flex: 1,

    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  username: {
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },

  external: {
    position: 'absolute',
    right: 0,

    fontSize: 22,
    color: colors.grayText,
  },

  divider: {
    height: 1,
    backgroundColor: colors.grayLight,
  },
});
