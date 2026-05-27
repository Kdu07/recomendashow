import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';

const tabs = [
  {
    key: 'explore',
    label: 'Explorar',
    icon: '⌂',
    iconSize: 28,
  },
  {
    key: 'search',
    label: 'Buscar',
    icon: '⌕',
    iconSize: 38,
  },
  {
    key: 'wishlist',
    label: 'Wishlist',
    icon: '♡',
    iconSize: 30,
  },
  {
    key: 'profile',
    label: 'Perfil',
    icon: '♙',
    iconSize: 36,
  },
];

export default function BottomTabBar({
  activeTab = 'explore',
  onTabPress,
}) {
  return (
    <View style={styles.container}>
      {tabs.map((tab) => {
        const isActive = tab.key === activeTab;

        return (
          <TouchableOpacity
            key={tab.key}
            style={styles.tab}
            onPress={() => onTabPress?.(tab.key)}
            activeOpacity={0.7}
          >
            <View style={styles.iconBox}>
              <Text
                style={[
                  styles.icon,
                  { fontSize: tab.iconSize },
                  isActive && styles.activeIcon,
                ]}
              >
                {tab.icon}
              </Text>
            </View>

            <Text style={[styles.label, isActive && styles.activeLabel]}>
              {tab.label}
            </Text>
          </TouchableOpacity>
        );
      })}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    height: 98,
    backgroundColor: colors.white,
    borderTopWidth: 2,
    borderTopColor: colors.grayLight,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-around',
    paddingBottom: spacing.xs,
  },

  tab: {
    alignItems: 'center',
    justifyContent: 'center',
    gap: 4,
    flex: 1,
  },

  iconBox: {
    width: 38,
    height: 34,
    alignItems: 'center',
    justifyContent: 'center',
  },

  icon: {
    lineHeight: 36,
    color: colors.grayText,
    textAlign: 'center',
  },

  activeIcon: {
    color: colors.purplePrimary,
    fontWeight: typography.fontWeight.semiBold,
  },

  label: {
    fontSize: typography.size.xs,
    color: colors.grayText,
  },

  activeLabel: {
    color: colors.purplePrimary,
    fontWeight: typography.fontWeight.semiBold,
  },
});