import React from 'react';

import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

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
      <View style={styles.content}>
        <View style={styles.iconContainer}>
          <Ionicons name={icon} size={21} color={colors.purplePrimary} />
        </View>

        <View style={styles.copy}>
          <Text style={styles.platform}>{platform}</Text>
          <Text style={styles.username}>{username}</Text>
        </View>

        <View style={styles.externalContainer}>
          <Ionicons name="open-outline" size={18} color={colors.grayText} />
        </View>
      </View>

      {showDivider && (
        <View style={styles.divider} />
      )}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.md,
  },

  content: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },

  iconContainer: {
    width: 40,
    height: 40,
    borderRadius: radius.pill,
    backgroundColor: colors.purpleSoft,
    alignItems: 'center',
    justifyContent: 'center',
  },

  copy: {
    flex: 1,
    gap: 2,
  },

  platform: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  username: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },

  externalContainer: {
    width: 32,
    height: 32,
    borderRadius: radius.pill,
    backgroundColor: colors.whiteSoft,
    alignItems: 'center',
    justifyContent: 'center',
  },

  divider: {
    height: 1,
    backgroundColor: colors.grayLight,
  },
});
