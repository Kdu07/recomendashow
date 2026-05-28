import React from 'react';

import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Image,
} from 'react-native';

import BottomTabBar from '../components/BottomTabBar';

import useProfileViewModel from '../viewmodels/useProfileViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

function MenuItem({
  icon,
  title,
  subtitle,
  onPress,
}) {
  return (
    <TouchableOpacity
      style={styles.menuItem}
      onPress={onPress}
      activeOpacity={0.8}
    >
      <View style={styles.menuLeft}>
        <View style={styles.iconContainer}>
          <Text style={styles.icon}>
            {icon}
          </Text>
        </View>

        <View>
          <Text style={styles.menuTitle}>
            {title}
          </Text>

          <Text style={styles.menuSubtitle}>
            {subtitle}
          </Text>
        </View>
      </View>

      <Text style={styles.chevron}>
        ›
      </Text>
    </TouchableOpacity>
  );
}

export default function ProfileScreen({ navigation }) {
  const {
    user,
    menuItems,

    handleMenuPress,
    handleEditProfile,
    handleLogout,
    handleTabPress,
  } = useProfileViewModel();

  return (
    <View style={styles.page}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        <Text style={styles.header}>
          PERFIL
        </Text>

        <View style={styles.profileSection}>
          <Image
            source={{ uri: user.avatar }}
            style={styles.avatar}
          />

          <View style={styles.profileInfo}>
            <Text style={styles.name}>
              {user.name}
            </Text>

            <Text style={styles.email}>
              {user.email}
            </Text>

            <TouchableOpacity
              onPress={handleEditProfile}
              activeOpacity={0.7}
            >
              <Text style={styles.editProfile}>
                ✎ Editar Perfil
              </Text>
            </TouchableOpacity>
          </View>
        </View>

        <View style={styles.menuList}>
          {menuItems.map((item) => (
            <MenuItem
              key={item.id}
              icon={item.icon}
              title={item.title}
              subtitle={item.subtitle}
              onPress={() => {
                if (item.id === 'preferences') {
                  navigation.navigate('EditInterests');
                  return;
                }
                handleMenuPress(item);
              }}
            />
          ))}
        </View>

        <TouchableOpacity
          style={styles.logoutButton}
          onPress={handleLogout}
          activeOpacity={0.7}
        >
          <Text style={styles.logoutIcon}>
            ↪
          </Text>

          <Text style={styles.logoutText}>
            Sair
          </Text>
        </TouchableOpacity>

        <Text style={styles.version}>
          Versão 1.0.0
        </Text>
      </ScrollView>

      <BottomTabBar
        activeTab="profile"
        onTabPress={(tab) => {
          if (tab === 'explore') {
            navigation.navigate('Home');
            return;
          }
          handleTabPress(tab);
        }}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  page: {
    flex: 1,
    backgroundColor: '#FAFAFA',
  },

  scrollContent: {
    paddingTop: spacing.xl,
    paddingBottom: 120,
  },

  header: {
    paddingHorizontal: spacing.lg,
    marginBottom: spacing.lg,

    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.bold,

    color: colors.grayDark,
  },

  profileSection: {
    flexDirection: 'row',
    alignItems: 'center',

    paddingHorizontal: spacing.xl,
    marginBottom: spacing.xxl,
    gap: spacing.md,
  },

  avatar: {
    width: 84,
    height: 84,
    borderRadius: 42,
  },

  profileInfo: {
    flex: 1,
    gap: 2,
  },

  name: {
    fontSize: 32,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  email: {
    fontSize: typography.size.md,
    color: colors.grayText,
  },

  editProfile: {
    marginTop: spacing.sm,
    fontSize: typography.size.sm,
    color: colors.purplePrimary,
    fontWeight: typography.fontWeight.medium,
  },

  menuList: {
    paddingHorizontal: spacing.md,
    gap: spacing.md,
  },

  menuItem: {
    backgroundColor: colors.white,

    borderRadius: radius.lg,

    paddingHorizontal: spacing.md,
    paddingVertical: spacing.lg,

    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',

    shadowColor: '#000',
    shadowOpacity: 0.04,
    shadowRadius: 8,
    shadowOffset: {
      width: 0,
      height: 2,
    },

    elevation: 2,
  },

  menuLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },

  iconContainer: {
    width: 52,
    height: 52,
    borderRadius: 26,

    backgroundColor: '#F4ECFF',

    alignItems: 'center',
    justifyContent: 'center',
  },

  icon: {
    fontSize: 22,
    color: colors.purplePrimary,
  },

  menuTitle: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  menuSubtitle: {
    marginTop: 2,

    fontSize: typography.size.sm,
    color: colors.grayText,
  },

  chevron: {
    fontSize: 28,
    color: colors.grayText,
  },

  logoutButton: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'flex-end',

    marginTop: spacing.xxl,
    marginRight: spacing.xl,
    gap: spacing.xs,
  },

  logoutIcon: {
    fontSize: 24,
    color: colors.peachBurnt,
  },

  logoutText: {
    fontSize: typography.size.lg,
    color: colors.peachBurnt,
    fontWeight: typography.fontWeight.medium,
  },

  version: {
    marginTop: 140,

    textAlign: 'center',

    fontSize: typography.size.sm,
    color: colors.grayText,
  },
});