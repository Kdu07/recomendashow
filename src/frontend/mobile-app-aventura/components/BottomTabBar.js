import React from 'react';
import { View, Text, Pressable, StyleSheet } from 'react-native';
import { BlurView } from 'expo-blur';
import { Ionicons } from '@expo/vector-icons';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing, radius } from '../constants/spacing';

const tabs = [
  {
    key: 'explore',
    label: 'Explorar',
    icon: 'home-outline',
    activeIcon: 'home',
  },
  {
    key: 'search',
    label: 'Buscar',
    icon: 'search-outline',
    activeIcon: 'search',
  },
  {
    key: 'wishlist',
    label: 'Wishlist',
    icon: 'heart-outline',
    activeIcon: 'heart',
  },
  {
    key: 'profile',
    label: 'Perfil',
    icon: 'person-outline',
    activeIcon: 'person',
  },
];

export default function BottomTabBar({
  activeTab = 'explore',
  onTabPress,
}) {
  return (
    <View style={styles.safeArea}>
      <BlurView intensity={35} tint="light" style={styles.backdrop}>
        <View style={styles.container}>
          {tabs.map((tab) => {
            const isActive = tab.key === activeTab;

            return (
              <Pressable
                key={tab.key}
                onPress={() => onTabPress?.(tab.key)}
                style={({ pressed }) => [
                  styles.tab,
                  isActive && styles.activeTab,
                  pressed && styles.pressedTab,
                ]}
              >
                <Ionicons
                  name={isActive ? tab.activeIcon : tab.icon}
                  size={22}
                  color={isActive ? colors.purplePrimary : colors.grayText}
                />

                <Text style={[styles.label, isActive && styles.activeLabel]}>
                  {tab.label}
                </Text>
              </Pressable>
            );
          })}
        </View>
      </BlurView>
    </View>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    position: 'absolute',
    left: 0,
    right: 0,
    bottom: 0,
    height: 32,
    backgroundColor: 'rgba(248, 250, 252, 0.86)',
    justifyContent: 'center',
    paddingHorizontal: spacing.md,
    paddingBottom: spacing.md,
  },

  backdrop: {
    width: '100%',
    borderRadius: radius.xl,
    overflow: 'hidden',

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 8,
    },
    shadowOpacity: 0.12,
    shadowRadius: 18,
    elevation: 10,
  },

  container: {
    height: 72,
    borderRadius: radius.xl,
    backgroundColor: 'rgba(255, 255, 255, 0.90)',
    borderWidth: 1,
    borderColor: 'rgba(224, 212, 255, 0.85)',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: spacing.sm,
  },

  tab: {
    flex: 1,
    height: 54,
    borderRadius: radius.lg,
    alignItems: 'center',
    justifyContent: 'center',
    gap: 3,
  },

  activeTab: {
    backgroundColor: colors.purpleSoft,
    borderWidth: 1,
    borderColor: colors.purpleLight,
  },

  pressedTab: {
    opacity: 0.88,
    transform: [{ scale: 0.98 }],
  },

  label: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayText,
  },

  activeLabel: {
    color: colors.purplePrimary,
    fontWeight: typography.fontWeight.semiBold,
  },
});