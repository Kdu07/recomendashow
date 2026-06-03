import React from 'react';

import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Image,
  SafeAreaView,
} from 'react-native';

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

        <View style={styles.menuCopy}>
          <Text style={styles.menuTitle}>
            {title}
          </Text>

          <Text style={styles.menuSubtitle}>
            {subtitle}
          </Text>
        </View>
      </View>

      <Text style={styles.chevron}>
        &gt;
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
  } = useProfileViewModel(navigation);

  return (
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.page}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <Text style={styles.header}>
            Perfil
          </Text>

          <View style={styles.profileCard}>
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
                  Editar perfil
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

                  if (item.id === 'notifications') {
                    navigation.navigate('Notifications');
                    return;
                  }

                  if (item.id === 'institutional') {
                    navigation.navigate('Institutional');
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
            <Text style={styles.logoutText}>
              Sair
            </Text>
          </TouchableOpacity>

          <Text style={styles.version}>
            Versão 1.0.0
          </Text>
        </ScrollView>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  page: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  scrollContent: {
    paddingHorizontal: spacing.md,
    paddingTop: spacing.md,
    paddingBottom: 140,
    gap: spacing.lg,
  },

  header: {
    fontSize: typography.size.xl,
    lineHeight: typography.lineHeight.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  profileCard: {
    flexDirection: 'row',
    alignItems: 'center',

    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,

    padding: spacing.lg,
    gap: spacing.md,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.04,
    shadowRadius: 10,
    elevation: 2,
  },

  avatar: {
    width: 84,
    height: 84,
    borderRadius: 42,
  },

  profileInfo: {
    flex: 1,
    gap: spacing.xs,
  },

  name: {
    fontSize: typography.size.xl,
    lineHeight: typography.lineHeight.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  email: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },

  editProfile: {
    marginTop: spacing.xs,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.purplePrimary,
    fontWeight: typography.fontWeight.semiBold,
  },

  menuList: {
    gap: spacing.md,
  },

  menuItem: {
    backgroundColor: colors.white,

    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,

    paddingHorizontal: spacing.md,
    paddingVertical: spacing.md,

    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',

    shadowColor: colors.black,
    shadowOpacity: 0.04,
    shadowRadius: 10,
    shadowOffset: {
      width: 0,
      height: 4,
    },

    elevation: 2,
  },

  menuLeft: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },

  menuCopy: {
    flex: 1,
  },

  iconContainer: {
    width: 52,
    height: 52,
    borderRadius: 26,

    backgroundColor: colors.purpleSoft,

    alignItems: 'center',
    justifyContent: 'center',
  },

  icon: {
    fontSize: 22,
    color: colors.purplePrimary,
  },

  menuTitle: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  menuSubtitle: {
    marginTop: 2,

    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },

  chevron: {
    fontSize: 28,
    color: colors.grayText,
  },

  logoutButton: {
    alignSelf: 'flex-end',

    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.md,
  },

  logoutText: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    color: colors.peachBurnt,
    fontWeight: typography.fontWeight.semiBold,
  },

  version: {
    marginTop: spacing.xxl,

    textAlign: 'center',

    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },
});
